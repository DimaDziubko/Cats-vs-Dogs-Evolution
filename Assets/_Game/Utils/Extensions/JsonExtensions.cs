using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Utils.Extensions
{
    public static class JsonExtensions
    {
        public static TimelineConfig ForTimeline(this JObject jsonData, int timelineId = 0) => 
            Generate(jsonData, timelineId);

        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig()
            {
                CurrentTimeline = Generate(jsonData, timelineId),
                BattleSpeedConfigs = ExtractBattleSpeedConfig(jsonData),
                CommonConfig = ExtractCommonConfig(jsonData),
                FoodBoostConfig = ExtractFoodBoostConfig(jsonData),
                ShopConfig = ExtractShopConfig(jsonData),
                FreeGemsPackDayConfig = ExtractFreeGemsPackDayConfig(jsonData),
                AdsConfig = ExtractAdsConfig(jsonData),
                GeneralDailyTaskConfig = ExtractGeneralDailyTaskConfig(jsonData),
            };
            //TODO Delete later
            Debug.Log("GAME CONFIG PARSED SUCCESSFULLY");
    
            return config;
        }

        private static List<WarriorConfig> ExtractWarriors(JObject jsonData)
        {
            var warriorsToken = jsonData[Constants.ConfigKeys.WARRIORS];
            if (warriorsToken == null)
            {
                Debug.LogError("WarriorsConfig is null");
                return null;
            }
            
            return warriorsToken.Select(warriorToken => warriorToken.ToObject<WarriorConfig>()).ToList();
        }

        private static List<RemoteAgeConfig> ExtractAges(JObject jsonData)
        {
            var agesToken = jsonData[Constants.ConfigKeys.AGES];
            if (agesToken == null)
            {
                Debug.LogError("AgeConfig is null");
                return null;
            }
            
            return agesToken.Select(ageToken => ageToken.ToObject<RemoteAgeConfig>()).ToList();
        }

        private static List<RemoteBattleConfig> ExtractBattles(JObject jsonData)
        {
            var battlesToken = jsonData[Constants.ConfigKeys.BATTLES];
            if (battlesToken == null)
            {
                Debug.LogError("BattleConfig is null");
                return null;
            }
            
            return battlesToken.Select(battleToken => battleToken.ToObject<RemoteBattleConfig>()).ToList();
        }

        private static AdsConfig ExtractAdsConfig(JObject jsonData)
        {
            var adsConfigToken = jsonData[Constants.ConfigKeys.ADS_CONFIG];
            if (adsConfigToken == null)
            {
                Debug.LogError("Ads config is null");
                return null;
            }
            return adsConfigToken.ToObject<AdsConfig>();
        }

        private static GeneralDailyTaskConfig ExtractGeneralDailyTaskConfig(JObject jsonData)
        {
            var generalDailyTaskToken = jsonData[Constants.ConfigKeys.GENERAL_DAILY_TASK_CONFIG];
            if (generalDailyTaskToken == null)
            {
                Debug.LogError("GeneralDailyTaskConfig is null");
                return null;
            }
            return generalDailyTaskToken.ToObject<GeneralDailyTaskConfig>();
        }

        private static FreeGemsPackDayConfig ExtractFreeGemsPackDayConfig(JObject jsonData)
        {
            var freeGemsPackDayConfigToken = jsonData[Constants.ConfigKeys.FREE_GEMS_PACK_DAY_CONFIG];
            if (freeGemsPackDayConfigToken == null)
            {
                Debug.LogError("FreeGemsPackDayConfig is null");
                return null;
            }
            return freeGemsPackDayConfigToken.ToObject<FreeGemsPackDayConfig>();
        }

        private static ShopConfig ExtractShopConfig(JObject jsonData)
        {
            var shopToken = jsonData[Constants.ConfigKeys.SHOP_CONFIG];
            if (shopToken == null)
            {
                Debug.LogError("ShopConfig is null");
                return null;
            }
            return shopToken.ToObject<ShopConfig>();
        }

        private static FoodBoostConfig ExtractFoodBoostConfig(JObject jsonData)
        {
            var foodBoostToken = jsonData[Constants.ConfigKeys.FOOD_BOOST_CONFIG];
            if (foodBoostToken == null)
            {
                Debug.LogError("FoodBoostConfig is null");
                return null;
            }
            return foodBoostToken.ToObject<FoodBoostConfig>();
        }

        private static CommonConfig ExtractCommonConfig(JObject jsonData) => 
            Resources.Load<CommonConfig>(Constants.LocalConfigPath.COMMON_CONFIG_PATH);

        private static List<BattleSpeedConfig> ExtractBattleSpeedConfig(JObject jsonData)
        {
            var battleSpeedTokens = jsonData[Constants.ConfigKeys.BATTLE_SPEEDS_CONFIGS];
            if (battleSpeedTokens == null)
            {
                Debug.LogError("BattleSpeedConfig is null");
                return null;
            }
            
            return battleSpeedTokens.Select(battleSpeedToken => battleSpeedToken.ToObject<BattleSpeedConfig>()).ToList();
        }

        private static TimelineConfig Generate(JObject jsonData, int timelineId)
        {
            TimelineConfig timelineConfig = new TimelineConfig
            {
                Id = timelineId, 
                Ages = new List<AgeConfig>(), 
                Battles = new List<BattleConfig>()
            };

            List<RemoteAgeConfig> remoteAgesData = ExtractAges(jsonData);
            GeneralAgeConfig ages = Resources.Load<GeneralAgeConfig>(Constants.LocalConfigPath.GENERAL_AGE_CONFIG_PATH);

            List<RemoteBattleConfig> remoteBattlesData = ExtractBattles(jsonData);
            GeneralBattleConfig battles = Resources.Load<GeneralBattleConfig>(Constants.LocalConfigPath.GENERAL_BATTLE_CONFIG_PATH);


            List<WarriorConfig> warriors = ExtractWarriors(jsonData);

            for (int level = 1; level <= 6; level++)
            {
                List<AgeConfig> ageLevelGroup = ages.AgeConfigs.Where(x => x.Level == level).ToList();
                List<BattleConfig> battleLevelGroup = battles.BattleConfigs.Where(x => x.Level == level).ToList();

                int index = (timelineId < ageLevelGroup.Count) ? timelineId : (timelineId + level - 1) % ageLevelGroup.Count;
        
                AgeConfig age = ageLevelGroup[index];
                BattleConfig battle = battleLevelGroup[index];
                
                var remoteAgeData = remoteAgesData.First(x => x.Id == age.Id);
                age.Economy = remoteAgeData.Economy;
                age.WarriorsId = remoteAgeData.WarriorsId;
                age.GemsPerAge = remoteAgeData.GemsPerAge;
                age.Price = remoteAgeData.Price;
                
                var remoteBattleData = remoteBattlesData.First(x => x.Id == battle.Id);
                battle.Scenario = remoteBattleData.Scenario;
                battle.WarriorsId = remoteBattleData.WarriorsId;
                battle.CoinsPerBase = remoteBattleData.CoinsPerBase;
                battle.MaxCoinsPerBattle = remoteBattleData.MaxCoinsPerBattle;
                battle.EnemyBaseHealth = remoteBattleData.EnemyBaseHealth;

                var warriorsForAgeAndBattle = age.WarriorsId
                    .Select(id => warriors.FirstOrDefault(w => w.Id == id))
                    .Where(warrior => warrior != null)
                    .ToList();

                age.Warriors = warriorsForAgeAndBattle;
                battle.Warriors = warriorsForAgeAndBattle;

                timelineConfig.Ages.Add(age);
                timelineConfig.Battles.Add(battle);
            }

            return timelineConfig;
        }
    }
}