using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using Assets._Game.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Utils.Extensions
{
    public static class JsonExtensions
    {
      public static TimelineConfig ForTimeline(this JObject jsonData, int timelineId = 0) => ExtractTimeline(jsonData, timelineId);

        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig()
            {
                Timeline = ExtractTimeline(jsonData, timelineId),
                BattleSpeedConfigs = ExtractBattleSpeedConfig(jsonData),
                CommonConfig = ExtractCommonConfig(jsonData),
                FoodBoostConfig = ExtractFoodBoostConfig(jsonData),
            };
            
            //TODO Delete later
            Debug.Log("GAME CONFIG PARSED SUCCESSFULLY");
    
            return config;
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
        
    }
}