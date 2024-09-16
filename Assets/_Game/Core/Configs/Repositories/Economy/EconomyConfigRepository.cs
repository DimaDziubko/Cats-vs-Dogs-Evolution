using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Services.UserContainer;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core.UserState;

namespace _Game.Core.Configs.Repositories.Economy
{
    public class EconomyConfigRepository : IEconomyConfigRepository
    {
        private readonly IUserContainer _userContainer;
        private readonly IAgeConfigRepository _ageAgeConfigRepository;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public EconomyConfigRepository(
            IUserContainer userContainer,
            IAgeConfigRepository ageConfigRepository)
        {
            _userContainer = userContainer;
            _ageAgeConfigRepository = ageConfigRepository;
        }
        
        public FoodBoostConfig GetFoodBoostConfig() => 
            _userContainer
                .GameConfig
                .FoodBoostConfig;

        public FreeGemsPackDayConfig GetFreeGemsPackDayConfig() => 
            _userContainer.GameConfig.FreeGemsPackDayConfig;

        public float GetMinimalCoinsForBattle() => 
            _ageAgeConfigRepository
                .GetAgeConfig(TimelineState.AgeId)
                .Economy
                .CoinPerBattle;

        public UpgradeItemConfig GetConfigForType(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return _ageAgeConfigRepository
                        .GetAgeConfig(TimelineState.AgeId)?
                        .Economy
                        .FoodProduction;;
                case UpgradeItemType.BaseHealth:
                    return _ageAgeConfigRepository
                        .GetAgeConfig(TimelineState.AgeId)?
                        .Economy
                        .BaseHealth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public int GetInitialFoodAmount() =>
            _ageAgeConfigRepository
                .GetAgeConfig(TimelineState.AgeId)
                .Economy.InitialFoodAmount;
    }
}