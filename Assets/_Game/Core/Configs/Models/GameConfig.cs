using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class GameConfig
    {
        public int TimelinesCount;
        public TimelineConfig CurrentTimeline;
        public CommonConfig CommonConfig;
        public FoodBoostConfig FoodBoostConfig;
        public List<BattleSpeedConfig> BattleSpeedConfigs;
        public ShopConfig ShopConfig;
        public FreeGemsPackDayConfig FreeGemsPackDayConfig;
        public AdsConfig AdsConfig;
        public GeneralDailyTaskConfig GeneralDailyTaskConfig;
    }
}