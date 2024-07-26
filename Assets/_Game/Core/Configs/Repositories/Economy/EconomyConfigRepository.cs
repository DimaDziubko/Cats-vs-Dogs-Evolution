using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Configs.Repositories.Economy
{
    public class EconomyConfigRepository : IEconomyConfigRepository
    {
        private readonly IUserContainer _persistentData;
        private readonly IAgeConfigRepository _ageConfigRepository;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public EconomyConfigRepository(
            IUserContainer persistentData,
            IAgeConfigRepository ageConfigRepository)
        {
            _persistentData = persistentData;
            _ageConfigRepository = ageConfigRepository;
        }
        
        public FoodBoostConfig GetFoodBoostConfig() => 
            _persistentData
                .GameConfig
                .FoodBoostConfig;

        public float GetMinimalCoinsForBattle() => 
            _ageConfigRepository
                .GetAgeConfig(TimelineState.AgeId)
                .Economy
                .CoinPerBattle;

        public UpgradeItemConfig GetConfigForType(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return _ageConfigRepository
                        .GetAgeConfig(TimelineState.AgeId)?
                        .Economy
                        .FoodProduction;;
                case UpgradeItemType.BaseHealth:
                    return _ageConfigRepository
                        .GetAgeConfig(TimelineState.AgeId)?
                        .Economy
                        .BaseHealth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public int GetInitialFoodAmount() =>
            _ageConfigRepository
                .GetAgeConfig(TimelineState.AgeId)
                .Economy.InitialFoodAmount;
    }
}