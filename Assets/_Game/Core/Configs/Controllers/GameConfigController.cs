using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Controllers
{
    public class GameConfigController : IGameConfigController
    {
        private readonly IPersistentDataService _persistentData;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public GameConfigController(IPersistentDataService persistentData)
        {
            _persistentData = persistentData;
        }
        
        private TimelineConfig GetCurrentTimeline()
        {
            int currentTimelineId = TimelineState.TimelineId;
            return _persistentData.GameConfig.Timeline.Id == currentTimelineId ? _persistentData.GameConfig.Timeline : null;
        }

        public AgeConfig GetAgeConfig(int ageId)
        {
            return GetCurrentTimeline()?.Ages
                .FirstOrDefault(age => age.Id == ageId);
        }

        public BattleConfig GetBattleConfig(int index)
        {
            return GetCurrentTimeline()?.Battles[index];
        }

        public int LastBattle()
        {
            return (GetCurrentTimeline().Battles.Count - 1);
        }

        public int LastAge()
        {
            return (GetCurrentTimeline().Ages.Count - 1);
        }
        
        public float GetAgePrice(int ageId)
        {
            var ageConfig = GetCurrentTimeline()?.Ages
                .FirstOrDefault(age => age.Id == ageId);

            if (ageConfig != null) return ageConfig.Price;
            return 0;
        }
        
        public List<WarriorConfig> GetCurrentAgeUnits()
        {
            return GetCurrentTimeline()?.Ages[TimelineState.AgeId].Warriors;
        }

        public List<WarriorConfig> GetEnemyConfigs(in int currentBattleIndex)
        {
            return GetCurrentTimeline()?.Battles[currentBattleIndex].Enemies;
        }

        public FoodProductionConfig GetFoodProduction()
        {
            return GetCurrentTimeline()?.Ages[TimelineState.AgeId].Economy.FoodProduction;
        }

        public List<WarriorConfig> GetOpenPlayerUnitConfigs()
        {
            var warriors = GetCurrentTimeline()?.Ages[TimelineState.AgeId].Warriors;

            if (warriors != null && TimelineState.OpenUnits != null)
            {
                var openWarriors = warriors
                    .Where(warrior => TimelineState.OpenUnits.Contains(warrior.Type))
                    .ToList();

                return openWarriors;
            }

            return new List<WarriorConfig>();
        }

        public float GetUnitPrice(in UnitType type)
        {
            var timeline = GetCurrentTimeline();

            var ageConfig = timeline?.Ages.FirstOrDefault(age => age.Id == TimelineState.AgeId);
            if (ageConfig == null) return 0;

            var unitType = type;
            var warrior = ageConfig.Warriors.FirstOrDefault(x => x.Type == unitType);
            return warrior?.Price ?? 0;
        }

        public BaseHealthConfig GetBaseHealthConfig()
        {
            return GetCurrentTimeline()?.Ages[TimelineState.AgeId].Economy.BaseHealth;
        }

        public string GetFoodIconKey()
        {
            return _persistentData.GameConfig.CommonConfig.FoodIconKey;
        }

        public string GetBaseIconKey()
        {
            return _persistentData.GameConfig.CommonConfig.BaseIconKey;
        }
    }

    public interface IGameConfigController
    {
        AgeConfig GetAgeConfig(int ageId);
        BattleConfig GetBattleConfig(int battleIndex);
        int LastBattle();
        float GetAgePrice(int timelineStateAgeId);
        List<WarriorConfig> GetCurrentAgeUnits();
        List<WarriorConfig> GetEnemyConfigs(in int currentBattleIndex);
        FoodProductionConfig GetFoodProduction();
        List<WarriorConfig> GetOpenPlayerUnitConfigs();
        string GetFoodIconKey();
        float GetUnitPrice(in UnitType type);
        BaseHealthConfig GetBaseHealthConfig();
        string GetBaseIconKey();
        int LastAge();
    }
}