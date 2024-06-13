﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Data.Age.Static._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Extensions;

namespace _Game.Core.DataPresenters._UpgradeItemPresenter
{
    public class UpgradeItemPresenter : IUpgradeItemPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<UpgradeItemModel> UpgradeItemUpdated;

        IEnumerable<Window> IUpgradeAvailabilityProvider.AffectedWindows
        {
            get
            {
                yield return Window.Upgrades;
                yield return Window.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable => 
            _models.Any(x => x.Value.CanAfford);
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGeneralDataPool _dataPool;
        private readonly IAgeNavigator _ageNavigator;


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
            IAgeNavigator ageNavigator)
        {
            _userContainer = userContainer;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _dataPool = dataPool;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            gameInitializer.OnMainInitialization += Init;
        }

        void IUpgradeItemPresenter.OnUpgradesWindowOpened() 
            => UpdateUpgradeItems();

        private void Init()
        {
            _upgradesChecker.Register(this);
            CreateUpgradeItems();
            UpdateUpgradeItems();
            SubscribeToEvents();
        }

        public void UpgradeItem(UpgradeItemType type, float price)
        {
            _logger.Log($"UpgradeItem {type}");
            _userContainer.UpgradeItem(type, price);
        }

        private void SubscribeToEvents()
        {
            UpgradeItems.Changed += OnUpgradeItemChanged;
            Currency.CoinsChanged += OnCoinsChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            UpgradeItems.Changed -= OnUpgradeItemChanged;
            Currency.CoinsChanged -= OnCoinsChanged;
            _gameInitializer.OnMainInitialization -= Init;
            _ageNavigator.AgeChanged -= OnAgeChanged;
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
                CanAfford = Currency.Coins >= dynamicData.Price,
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
            model.CanAfford = Currency.Coins >= dynamicData.Price;
            UpgradeItemUpdated?.Invoke(model);
        }
        
        private void OnCoinsChanged(bool isPositive) => 
            UpdateUpgradeItems();

        private void Cleanup() => _models.Clear();
    }
}