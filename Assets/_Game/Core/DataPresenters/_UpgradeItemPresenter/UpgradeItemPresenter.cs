using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Data.Age.Static._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Core._Logger;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.UserState;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.DataPresenters._UpgradeItemPresenter
{
    public class UpgradeItemPresenter : IUpgradeItemPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<UpgradeItemModel> UpgradeItemUpdated;

        IEnumerable<Screen> IUpgradeAvailabilityProvider.AffectedScreens
        {
            get
            {
                yield return Screen.Upgrades;
                yield return Screen.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable => 
            _models.Any(x => x.Value.ButtonState == ButtonState.Active);
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGeneralDataPool _dataPool;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IRaceChanger _raceChanger;

        private readonly Dictionary<UpgradeItemType, UpgradeItemModel> _models = 
            new Dictionary<UpgradeItemType, UpgradeItemModel>(2);

        private IUserCurrenciesStateReadonly Currency => _userContainer.State.Currencies;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IUpgradeItemsReadonly UpgradeItems => _dataPool.AgeDynamicData.UpgradeItems;
        
        public UpgradeItemPresenter(
            IUserContainer userContainer,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGeneralDataPool dataPool,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            IRaceChanger raceChanger)
        {
            _userContainer = userContainer;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _dataPool = dataPool;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _raceChanger = raceChanger;
            
            gameInitializer.OnMainInitialization += Init;
        }

        void IUpgradeItemPresenter.OnUpgradesScreenOpened() 
            => UpdateUpgradeItems();

        private void Init()
        {
            _upgradesChecker.Register(this);
            CreateUpgradeItems();
            UpdateUpgradeItems();
            SubscribeToEvents();
        }

        public void UpgradeItem(UpgradeItemType type, float price) => 
            _userContainer.UpgradeStateHandler.UpgradeItem(type, price);

        private void SubscribeToEvents()
        {
            UpgradeItems.Changed += OnUpgradeItemChanged;
            Currency.CurrenciesChanged += OnCurrenciesChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _raceChanger.RaceChanged += OnRaceChanged;
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            UpgradeItems.Changed -= OnUpgradeItemChanged;
            Currency.CurrenciesChanged -= OnCurrenciesChanged;
            _gameInitializer.OnMainInitialization -= Init;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _raceChanger.RaceChanged -= OnRaceChanged;
        }

        private void OnRaceChanged()
        {
            Cleanup();
            CreateUpgradeItems();
            UpdateUpgradeItems();
        }

        private void OnAgeChanged()
        {
            Cleanup();
            CreateUpgradeItems();
            UpdateUpgradeItems();
        }

        private void OnUpgradeItemChanged(UpgradeItemType type, UpgradeItemDynamicData value) => 
            UpdateUpgradeItemModel(type, value);
        

        private void CreateUpgradeItems()
        {
            foreach (var type in Enum.GetValues(typeof(UpgradeItemType)))
            {
                _models[(UpgradeItemType)type] = CreateUpgradeItemModel((UpgradeItemType)type);
            }
        }

        private UpgradeItemModel CreateUpgradeItemModel(UpgradeItemType type)
        {
            var dynamicData = UpgradeItems.GetItemData(type);
            
            return new UpgradeItemModel
            {
                StaticData = new UpgradeItemStaticData
                {
                    Icon = type == UpgradeItemType.FoodProduction 
                        ? _dataPool.AgeStaticData.ForFoodIcon(RaceState.CurrentRace)
                        : _dataPool.AgeStaticData.ForTowerIcon(),
                    Type = type,
                    Name = type == UpgradeItemType.FoodProduction ? "Food Production" : "Base Health"
                },
                
                DynamicData = dynamicData,
                
                AmountText =  type == UpgradeItemType.FoodProduction
                    ? dynamicData.Amount.ToSpeedFormat() 
                    : dynamicData.Amount.FormatMoney(),
                
                ButtonState = Currency.Coins >= dynamicData.Price 
                    ? ButtonState.Active 
                    : ButtonState.Inactive,
            };
        }
        
        private void UpdateUpgradeItems()
        {
            foreach (var type in _models.Keys)
            {
                var dynamicData = UpgradeItems.GetItemData(type);
                UpdateUpgradeItemModel(type, dynamicData);
            }
        }

        private void UpdateUpgradeItemModel(UpgradeItemType type, UpgradeItemDynamicData dynamicData)
        {
            var model = _models[type];
            model.DynamicData = dynamicData;
            model.AmountText = type == UpgradeItemType.FoodProduction
                ? dynamicData.Amount.ToSpeedFormat()
                : dynamicData.Amount.FormatMoney();
            model.ButtonState = Currency.Coins >= dynamicData.Price 
                ? ButtonState.Active
                : ButtonState.Inactive;
            UpgradeItemUpdated?.Invoke(model);
        }
        
        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source) => 
            UpdateUpgradeItems();

        private void Cleanup() => _models.Clear();
    }
}