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

        private static List<AgeConfig> ExtractAges(JObject jsonData)
        {
            var agesToken = jsonData[Constants.ConfigKeys.AGES];
            if (agesToken == null)
            {
                Debug.LogError("AgeConfig is null");
                return null;
            }
            
            return agesToken.Select(ageToken => ageToken.ToObject<AgeConfig>()).ToList();
        }

        private static List<BattleConfig> ExtractBattles(JObject jsonData)
        {
            var battlesToken = jsonData[Constants.ConfigKeys.BATTLES];
            if (battlesToken == null)
            {
                Debug.LogError("BattleConfig is null");
                return null;
            }
            
            return battlesToken.Select(battleToken => battleToken.ToObject<BattleConfig>()).ToList();
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

        private static CommonConfig ExtractCommonConfig(JObject jsonData)
        {
            var commonToken = jsonData[Constants.ConfigKeys.COMMON_CONFIG];
            if (commonToken == null)
            {
                Debug.LogError("CommonConfig is null");
                return null;
            }
            return commonToken.ToObject<CommonConfig>();
        }

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

            List<AgeConfig> ages = ExtractAges(jsonData);
            List<BattleConfig> battles = ExtractBattles(jsonData);
            List<WarriorConfig> warriors = ExtractWarriors(jsonData);
            
            List<int> ageIds = new List<int>();
            List<int> battleIds = new List<int>();
            
            for (int i = 0; i < 6; i++)
            {
                List<AgeConfig> ageLevelGroup = ages.Where(x => x.Level == i+1).ToList();
                List<BattleConfig> battleLevelGroup = battles.Where(x => x.Level == i+1).ToList();
                
                int index = 0;
                if (timelineId >= ageLevelGroup.Count)
                {
                    index = (timelineId + i) % ageLevelGroup.Count;
                }
                else
                {
                    index = timelineId;
                }
                
                var age = ageLevelGroup[index];
                var battle = battleLevelGroup[index];

                int idOffset = 1;
                
                battle.Warriors = age.Warriors = age.WarriorsId
                    .Select(id => warriors.FirstOrDefault(w => w.Id == (id - idOffset)))
                    .Where(warrior => warrior != null)
                    .ToList();
                
                ageIds.Add(age.Id);
                battleIds.Add(battle.Id);
                
                timelineConfig.Ages.Add(age);
                timelineConfig.Battles.Add(battle);
            }
            
            Debug.Log($"TimelineId: {timelineId}");
            Debug.Log("Ages: [" + string.Join(", ", ageIds) + "]");
            Debug.Log("Battles: [" + string.Join(", ", battleIds) + "]");


            return timelineConfig;
        }
    }
}