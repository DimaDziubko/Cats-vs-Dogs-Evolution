using System;
using System.Linq;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

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

        public int LastBattleIndex()
        {
            return (GetCurrentTimeline().Battles.Count - 1);
        }

        public string GetBattlesKey()
        {
            return GetCurrentTimeline()?.BattleAssetKey;
        }

        public float GetAgePrice(int ageId)
        {
            var ageConfig = GetCurrentTimeline()?.Ages
                .FirstOrDefault(age => age.Id == ageId);

            if (ageConfig != null) return ageConfig.Price;
            return 0;
        }

        public string GetUnitsKey()
        {
            var ageConfig = GetCurrentTimeline()?.Ages.FirstOrDefault(age => age.Id == TimelineState.AgeId);
            if (ageConfig != null) return ageConfig.UnitAssetKey;
            else
            {
                throw new Exception("There is not unit asset key");
            }
        }
        
        public string[] GetUnitNames()
        {
            var currentAge = GetCurrentTimeline()?.Ages.FirstOrDefault(age => age.Id == TimelineState.AgeId);
            if (currentAge != null && currentAge.Warriors.Count > 0)
            {
                return currentAge.Warriors.Select(warrior => warrior.Name).ToArray();
            }
            else
            {
                throw new Exception("There are no warriors in the current age.");
            }
        }

        public float GetUnitPrice(in int unitIndex)
        {
            var economy = GetCurrentTimeline()?.Ages.FirstOrDefault(age => age.Id == TimelineState.AgeId)?.Economy;
            
            if (economy != null) return economy.WarriorPrices[unitIndex];
            else
            {
                throw new Exception("There is not economy");
            }
        }

        public BattleScenario GetBattleScenario(in int currentBattleIndex)
        {
            var battles = GetCurrentTimeline()?.Battles;
            return battles?[currentBattleIndex].Scenario;
        }

        public string GetEnemiesKey(int battleIndex)
        {
            var battleConfig = GetCurrentTimeline()?.Battles[battleIndex];
            if (battleConfig != null) return battleConfig.EnemyAssetKey;
            else
            {
                throw new Exception("There is not enemyAsset");
            }
        }

        public WarriorConfig GetEnemyConfig(in int currentBattleIndex, in int enemyIndex)
        {
            var battleConfig = GetCurrentTimeline()?.Battles[currentBattleIndex];
            if (battleConfig != null) return battleConfig.Enemies[enemyIndex];
            else
            {
                throw new Exception("There is not enemyAsset");
            }
        }
    }

    public interface IGameConfigController
    {
        AgeConfig GetAgeConfig(int ageId);
        BattleConfig GetBattleConfig(int battleIndex);
        int LastBattleIndex();
        string GetBattlesKey();
        float GetAgePrice(int timelineStateAgeId);
        string GetUnitsKey();
        string[] GetUnitNames();
        float GetUnitPrice(in int unitIndex);
        BattleScenario GetBattleScenario(in int currentBattleIndex);
        string GetEnemiesKey(int battleIndex);
        WarriorConfig GetEnemyConfig(in int currentBattleIndex, in int enemyIndex);
    }
}