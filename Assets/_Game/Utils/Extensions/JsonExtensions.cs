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
            ExtractTimeline(jsonData, timelineId);

        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig()
            {
                CurrentTimeline = ExtractTimeline(jsonData, timelineId),
                BattleSpeedConfigs = ExtractBattleSpeedConfig(jsonData),
                CommonConfig = ExtractCommonConfig(jsonData),
                FoodBoostConfig = ExtractFoodBoostConfig(jsonData),
                TimelinesCount = ExtractTimelinesCount(jsonData),
                ShopConfig = ExtractShopConfig(jsonData),
                FreeGemsPackDayConfig = ExtractFreeGemsPackDayConfig(jsonData),
                AdsConfig = ExtractAdsConfig(jsonData)
            };
            
            //TODO Delete later
            Debug.Log("GAME CONFIG PARSED SUCCESSFULLY");
    
            return config;
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

        private static TimelineConfig ExtractTimeline(JObject jsonData, int timelineId)
        {
            var timelineToken = jsonData[Constants.ConfigKeys.TIMELINES]?
                .FirstOrDefault(t => ((int)t[Constants.ConfigKeys.ID]) == timelineId);
            
            if (timelineToken == null)
            {
               Debug.LogError("TimelineConfig is null");
               return null;
            }
            return timelineToken.ToObject<TimelineConfig>();
        }
        
        private static int ExtractTimelinesCount(JObject jsonData)
        {
            var timelinesToken = jsonData[Constants.ConfigKeys.TIMELINES];
            if (timelinesToken == null)
            {
                Debug.LogError("Timelines array is null");
                return 0;
            }
            return timelinesToken.Count();
        }
    }
}