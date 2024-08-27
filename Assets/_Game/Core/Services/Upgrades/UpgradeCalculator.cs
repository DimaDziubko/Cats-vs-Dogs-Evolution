using System;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core.Services.Upgrades
{
    public class UpgradeCalculator : IUpgradeCalculator, IDisposable
    {
        private readonly IEconomyConfigRepository _economyConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public  UpgradeCalculator(
            IConfigRepositoryFacade configRepositoryFacade, 
            IUserContainer userContainer,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _economyConfigRepository = configRepositoryFacade.EconomyConfigRepository;
            _userContainer = userContainer;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemLevelChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            UpdateUpgradeValues();
        }

        void IDisposable.Dispose()
        {
            TimelineState.UpgradeItemLevelChanged -= OnUpgradeItemLevelChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnAgeChanged() => UpdateUpgradeValues();

        private void UpdateUpgradeValues()
        { 
            UpdateUpgradeValue(UpgradeItemType.BaseHealth, TimelineState.BaseHealthLevel);
            UpdateUpgradeValue(UpgradeItemType.FoodProduction, TimelineState.FoodProductionLevel);
        }
        
        private void OnUpgradeItemLevelChanged(UpgradeItemType type, int level) => 
            UpdateUpgradeValue(type, level);

        private void UpdateUpgradeValue(UpgradeItemType type, int level)
        {
            UpgradeItemDynamicData newValue = new UpgradeItemDynamicData
            {
                Amount = CalculateAmountForType(type, level),
                Price = CalculatePriceForType(type, level)
            };

            _generalDataPool.AgeDynamicData.ChangeUpgradeItemValue(type, newValue);
        }

        private float CalculatePriceForType(UpgradeItemType type, int level)
        {
            var config = _economyConfigRepository.GetConfigForType(type);
            return CalculatePrice(config.Price, config.PriceExponential, level);
        }
        private float CalculateAmountForType(UpgradeItemType type, int level)
        {
            var config = _economyConfigRepository.GetConfigForType(type);
            return config.Value + config.ValueStep * level;
        }

        private float CalculatePrice(float basePrice, Exponential growth, int level) => 
            Mathf.Round(basePrice + growth.GetValue(level));
    }
}