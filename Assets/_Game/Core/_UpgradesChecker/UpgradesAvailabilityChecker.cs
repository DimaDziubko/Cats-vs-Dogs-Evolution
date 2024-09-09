using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Debugger;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.Core._UpgradesChecker
{
    public class UpgradesAvailabilityChecker : IUpgradesAvailabilityChecker, IDisposable
    {
        public event Action<NotificationData> Notify;

        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IAgeNavigator _ageNavigator;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;


        private IEnumerable<GameScreen> RelevantScreens { get; } = new List<GameScreen>()
        {
            GameScreen.Upgrades,
            GameScreen.Evolution,
            GameScreen.UpgradesAndEvolution,
            GameScreen.Shop,
            GameScreen.GeneralCards,
            GameScreen.Cards
        };

        private readonly List<IUpgradeAvailabilityProvider> _upgradeProviders 
            = new List<IUpgradeAvailabilityProvider>();

        private readonly Dictionary<GameScreen, NotificationData> _data
            = new Dictionary<GameScreen, NotificationData>(3);

        public NotificationData GetNotificationData(GameScreen gameScreen)
        {
            if (_data.TryGetValue(gameScreen, out var data)) return data;
            else
            {
                _logger.Log("Data doesn't contains key {}");
                return null;
            }
        }

        public UpgradesAvailabilityChecker(
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IUserContainer userContainer)
        {
            _logger = logger;
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            gameInitializer.OnPostInitialization += Init;
            //debugger.NotificationData = _data;
        }

        public void Register(IUpgradeAvailabilityProvider provider) => 
            _upgradeProviders.Add(provider);

        public void UnRegister(IUpgradeAvailabilityProvider provider) => 
            _upgradeProviders.Remove(provider);

        private void UpdateData()
        {
            ResetScreenAvailability();
            AggregateAvailabilityStates();
            NotifyUpdatedStates();
        }

        private void ResetScreenAvailability()
        {
            foreach (var screen in RelevantScreens)
            {
                if (!_data.ContainsKey(screen))
                {
                    _data[screen] = new NotificationData
                    {
                        GameScreen = screen,
                        IsAvailable = false,
                        IsReviewed = false
                    };
                }
                else
                {
                    _data[screen].IsAvailable = false;
                }
            }
        }

        private void AggregateAvailabilityStates()
        {
            foreach (var screen in RelevantScreens)
            {
                if (RelevantScreens.Contains(screen))
                {
                    _data[screen].IsAvailable = 
                        _upgradeProviders
                            .Where(p => p.AffectedScreens.Contains(screen))
                            .Any(p => p.IsAvailable);
                }
            }
        }
        
        private void NotifyUpdatedStates()
        {
            foreach (var window in RelevantScreens)
            {
                Notify?.Invoke(_data[window]);
            }
        }

        private void Init()
        {
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            UpdateData();
        }

        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source)
        {
            if (delta > 0)
            {
                ResetReviewed();
                UpdateData();
            }
            else
            {
                AggregateAvailabilityStates();
            }
        }
        
        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void ResetReviewed()
        {
            foreach (var item in _data)
            {
                item.Value.IsReviewed = false;
            }
        }

        public void MarkAsReviewed(GameScreen gameScreen)
        {
            if (gameScreen == GameScreen.UpgradesAndEvolution)
            {
                _data[gameScreen].IsReviewed = CheckAllUpgradesReviewed();
                Notify?.Invoke(_data[gameScreen]);
                return;
            }
            _data[gameScreen].IsReviewed = true;
            Notify?.Invoke(_data[gameScreen]);
        }

        private bool CheckAllUpgradesReviewed()
        {
            return (_data[GameScreen.Upgrades].IsReviewed && _data[GameScreen.Evolution].IsReviewed)
                   || (_data[GameScreen.Upgrades].IsReviewed && !_data[GameScreen.Evolution].IsAvailable)
                   || (!_data[GameScreen.Upgrades].IsAvailable && _data[GameScreen.Evolution].IsReviewed);
        }
    }
}