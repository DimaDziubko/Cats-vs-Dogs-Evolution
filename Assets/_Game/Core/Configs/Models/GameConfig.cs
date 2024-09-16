using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Core.Configs.Models
{
    public class GameConfig
    {
        public TimelineConfig CurrentTimeline;
        public CommonConfig CommonConfig;
        public FoodBoostConfig FoodBoostConfig;
        public List<BattleSpeedConfig> BattleSpeedConfigs;
        public ShopConfig ShopConfig;
        public FreeGemsPackDayConfig FreeGemsPackDayConfig;
        public AdsConfig AdsConfig;
        public GeneralDailyTaskConfig GeneralDailyTaskConfig;
        public SummoningData SummoningData;
        public Dictionary<CardType, List<CardConfig>> CardConfigsByType;
        public Dictionary<int, CardConfig> CardConfigsById;
        public CardsPricingConfig CardPricingConfig;
    }
}