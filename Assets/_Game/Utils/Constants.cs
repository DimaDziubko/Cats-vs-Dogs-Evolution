using Pathfinding.RVO;

namespace _Game.Utils
{
    public sealed class Constants
    {
        public sealed class LocalConfigPath
        {
            public const string GENERAL_WARRIOR_CONFIG_PATH = "Warrior/GeneralWarriorsConfig";
            public const string COMMON_CONFIG_PATH = "Common/CommonConfig";
            public const string GENERAL_AGE_CONFIG_PATH = "Age/GeneralAgesConfig";
            public const string GENERAL_BATTLE_CONFIG_PATH = "Battle/GeneralBattlesConfig";
            public const string CARDS_CONFIG_PATH = "Card/CardsConfig";
            public const string SUMMONING_CONFIG_PATH = "Card/SummoningConfig";
            public const string CARDS_PRICING_PATH = "Card/CardsPricingConfig";
        }
        public sealed class Money
        {
            public const int MIN_COINS_PER_BATTLE = 9;
        }

        public sealed class CacheContext
        {
            public const int TIMELINE = 0;
            public const int AGE = 1;
            public const int BATTLE = 2;
            public const int GENERAL = 3;
        }

        public sealed class Scenes
        {
            public const string STARTUP = "Startup";
            public const string BATTLE_MODE = "BattleMode";
        }

        public sealed class SortingLayer
        {
            public const float SORTING_TRESHOLD = 0.1f;
            public const int SORTING_ORDER_MIN = -32768;
            public const int SORTING_ORDER_MAX = 32767;
        }

        public sealed class Layer
        {
            public const int PLAYER_PROJECTILE = 16;
            public const int ENEMY_PROJECTILE = 17;
            public const int MELEE_PLAYER = 8;
            public const int MELEE_ENEMY = 9;
            public const int ENEMY_BASE = 14;
            public const int PLAYER_BASE = 15;
            public const int PLAYER_AGGRO = 10;
            public const int ENEMY_AGGRO = 11;
            public const int PLAYER_ATTACK = 12;
            public const int ENEMY_ATTACK = 13;
            public const int RANGE_PLAYER = 19;
            public const int RANGE_ENEMY = 20;

            public const RVOLayer RVO_MELEE_ENEMY = RVOLayer.Layer3;
            public const RVOLayer RVO_RANGE_ENEMY = RVOLayer.Layer4;

            public const RVOLayer RVO_MELEE_PLAYER = RVOLayer.Layer5;
            public const RVOLayer RVO_RANGE_PLAYER = RVOLayer.Layer2;

        }

        public sealed class ComparisonThreshold
        {
            public const float MONEY_EPSILON = 0.01f;
            public const float UNIT_ROTATION_EPSILON = 0.05f;
        }
        
        public class TutorialSteps
        {
            public const int START_TUTORIAL_KEY = -1;
            
            public const int RACE_SELECTION = 1;
            public const int BUILDER = 2;
            public const int UPGRADE_SCREEN_BTN = 3;
            public const int FOOD_UPGRADE = 4;
            public const int EVOLVE = 5;
            public const int DAILY = 6;
            public const int CARDS_SCREEN_BTN = 7;
            public const int CARDS_PURCHASE = 8;
            
            public const int LAST_STEP = 8;
            public const int COMPLETE_TUTORIAL_KEY = -2;
        }

        public static class FeatureCompletedBattleThresholds
        {
            public const int BATTLE_SPEED = 10;
            public const int FOOD_BOOST = 2;
            public const int PAUSE = 1;
            public const int UPGRADES_SCREEN = 1;
            public const int EVOLUTION_SCREEN = 3;
            public const int X2 = 3;
            public const int SHOP = 6;
            public const int DAILY = 5;
        }

        public sealed class ConfigKeys
        {
            public const string WARRIORS = "Warriors";
            public const string AGES = "Ages";
            public const string BATTLES = "Battles";
            
            //Summoning
            public const string SUMMONING_CONFIGS = "SummoningConfigs";

            //Ads
            public const string ADS_CONFIG = "AdsConfig";

            //FreeGemsPackDayConfig
            public const string FREE_GEMS_PACK_DAY_CONFIG = "FreeGemsPackDayConfig";

            //BattleSpeed
            public const string BATTLE_SPEEDS_CONFIGS = "BattleSpeedConfigs";

            //FoodBoost
            public const string FOOD_BOOST_CONFIG = "FoodBoostConfig";

            //Common
            public const string COMMON_CONFIG = "CommonConfig";
            public const string MISSING_KEY = "-";

            //Timeline common
            public const string ID = "Id";

            //Timeline
            public const string TIMELINES = "Timelines";

            //Shop
            public const string SHOP_CONFIG = "ShopConfig";
            public const string PLACEMENT = "adPlacement";
            
            //GeneralDailyTask
            public const string GENERAL_DAILY_TASK_CONFIG = "GeneralDailyTaskConfig";
        }
    }
}