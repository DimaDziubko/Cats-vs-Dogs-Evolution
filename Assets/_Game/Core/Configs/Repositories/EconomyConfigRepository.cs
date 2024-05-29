using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

namespace _Game.Core.Configs.Repositories
{
    public class EconomyConfigRepository : IEconomyConfigRepository
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IAgeConfigRepository _ageConfigRepository;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public EconomyConfigRepository(
            IPersistentDataService persistentData,
            IAgeConfigRepository ageConfigRepository)
        {
            _persistentData = persistentData;
            _ageConfigRepository = ageConfigRepository;
        }

        public FoodProductionConfig GetFoodProduction() => 
            _ageConfigRepository
                .GetAgeConfig(TimelineState.AgeId)?
                .Economy
                .FoodProduction;

        public BaseHealthConfig GetBaseHealthConfig() => 
            _ageConfigRepository
                .GetAgeConfig(TimelineState.AgeId)?
                .Economy
                .BaseHealth;

        public string GetFoodIconKey() => 
            _persistentData
                .GameConfig
                .CommonConfig
                .FoodIconKey;

        public string GetBaseIconKey() => 
            _persistentData
                .GameConfig
                .CommonConfig
                .BaseIconKey;

        public FoodBoostConfig GetFoodBoostConfig() => 
            _persistentData
                .GameConfig
                .FoodBoostConfig;

        public float GetMinimalCoinsForBattle() => 
            _ageConfigRepository
                .GetAgeConfig(TimelineState.AgeId)
                .Economy
                .CoinPerBattle;
    }

    public interface IEconomyConfigRepository
    {
        FoodProductionConfig GetFoodProduction();
        BaseHealthConfig GetBaseHealthConfig();
        FoodBoostConfig GetFoodBoostConfig();
        float GetMinimalCoinsForBattle();
        string GetFoodIconKey();
        string GetBaseIconKey();
    }
}