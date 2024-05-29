using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.UI.Pin.Scripts
{
    public class UpgradesAvailabilityChecker : IUpgradesAvailabilityChecker
    {
        public event Action<NotificationData> Notify;

        private readonly IPersistentDataService _persistentData;
        private readonly IMyLogger _logger;
        private IUserCurrenciesStateReadonly Currencies => _persistentData.State.Currencies;

        private readonly Dictionary<Window, NotificationData> _data = new Dictionary<Window, NotificationData>(3);
        
        public NotificationData GetNotificationData(Window window)
        {
            if (_data.TryGetValue(window, out var data)) return data;
            else
            {
                _logger.Log("Data doesn't contains key {}");
                return null;
            }
        }

        private bool AnyUpgradeIsAvailable => (_isAnyUpgradeUnitItemAvailable ||
                                               _isFoodUpgradeItemAvailable ||
                                               _isBaseUpgradeItemAvailable);

        private bool EvolutionOrTravelIsAvailable => (_isEvolutionAvailable ||
                                                      _isTravelAvailable);

        private bool EvolutionOrUpgradeIsAvailable => AnyUpgradeIsAvailable || 
                                                      EvolutionOrTravelIsAvailable;

        private bool _isAnyUpgradeUnitItemAvailable;
        private bool _isFoodUpgradeItemAvailable;
        private bool _isBaseUpgradeItemAvailable;
        private bool _isEvolutionAvailable;
        private bool _isTravelAvailable;

        public UpgradesAvailabilityChecker(
            IPersistentDataService persistentData, 
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _logger = logger;
        }

        public void Init()
        {
            UpdateAndNotifyAboutUpgrades();
            Currencies.CoinsChanged += OnCoinsChanged;
        }

        private void OnCoinsChanged(bool isPositive)
        {
            if (isPositive)
            {
                ResetReviewed();
                UpdateAndNotifyAboutUpgrades();
                return;
            }
            UpdateAndNotifyAboutUpgrades();
        }

        private void ResetReviewed()
        {
            foreach (var item in _data)
            {
                item.Value.IsReviewed = false;
            }
        }

        public void MarkAsReviewed(Window window)
        {
            if (window == Window.UpgradesAndEvolution)
            {
                _data[window].IsReviewed = CheckAllUpgradesReviewed();
                Notify?.Invoke(_data[window]);
                return;
            }
            _data[window].IsReviewed = true;
            Notify?.Invoke(_data[window]);
        }

        private bool CheckAllUpgradesReviewed()
        {
            return (_data[Window.Upgrades].IsReviewed && _data[Window.Evolution].IsReviewed)
                   || (_data[Window.Upgrades].IsReviewed && !_data[Window.Evolution].IsAvailable)
                   || (!_data[Window.Upgrades].IsAvailable && _data[Window.Evolution].IsReviewed);
        }

        public void OnUpgradeUnitItemsUpdated(List<UpgradeUnitItemViewModel> unitItems) => 
            _isAnyUpgradeUnitItemAvailable = unitItems.Any(x => x.CanAfford && !x.IsBought);

        public void OnUpgradeUnitItemsUpdated(UpgradeItemViewModel upgradeItem)
        {
            switch (upgradeItem.Type)
            {
                case UpgradeItemType.FoodProduction:
                    _isFoodUpgradeItemAvailable = upgradeItem.CanAfford;
                    break;
                case UpgradeItemType.BaseHealth:
                    _isBaseUpgradeItemAvailable = upgradeItem.CanAfford;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnEvolutionModelUpdated(EvolutionTabData model) => 
            _isEvolutionAvailable = model.EvolutionBtnData.CanAfford;

        public void OnTravelModelUpdated(TravelTabData model) => 
            _isTravelAvailable = model.CanTravel;

        private void UpdateAndNotifyAboutUpgrades()
        {
            UpdateAndNotifyUpgradesWindow();
            UpdateAndNotifyEvolutionWindow();
            UpdateAndNotifyUpgradesAndEvolutionWindow();
        }

        private void UpdateAndNotifyUpgradesAndEvolutionWindow()
        {
            if (_data.ContainsKey(Window.UpgradesAndEvolution))
            {
                _data[Window.UpgradesAndEvolution].Window = Window.UpgradesAndEvolution;
                _data[Window.UpgradesAndEvolution].IsAvailable = EvolutionOrUpgradeIsAvailable;
            }
            else
            {
                var data = new NotificationData()
                {
                    Window = Window.UpgradesAndEvolution, 
                    IsAvailable = EvolutionOrUpgradeIsAvailable,
                };
                
                _data.Add(Window.UpgradesAndEvolution, data);
            }
            Notify?.Invoke(_data[Window.UpgradesAndEvolution]);
        }

        private void UpdateAndNotifyEvolutionWindow()
        {
            if (_data.ContainsKey(Window.Evolution))
            {
                _data[Window.Evolution].Window = Window.Evolution;
                _data[Window.Evolution].IsAvailable = EvolutionOrTravelIsAvailable;
            }
            else
            {
                var data = new NotificationData()
                {
                    Window = Window.Upgrades, 
                    IsAvailable = EvolutionOrTravelIsAvailable,
                };
                
                _data.Add(Window.Evolution, data);
            }
            Notify?.Invoke(_data[Window.Evolution]);
        }

        private void UpdateAndNotifyUpgradesWindow()
        {
            if (_data.ContainsKey(Window.Upgrades))
            {
                _data[Window.Upgrades].Window = Window.Upgrades;
                _data[Window.Upgrades].IsAvailable = AnyUpgradeIsAvailable;
            }
            else
            {
                var data = new NotificationData()
                {
                    Window = Window.Upgrades, 
                    IsAvailable = AnyUpgradeIsAvailable,
                };
                
                _data.Add(Window.Upgrades, data);
            }
            Notify?.Invoke(_data[Window.Upgrades]);
        }
    }
}