using Pathfinding.RVO;

namespace _Game.Utils
{
    public sealed class Constants
    {
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

        public sealed class TutorialStepTreshold
        {
            public const int UNIT_BUILDER_BUTTON = 0;
            public const int UPGRADES_WINDOW = 1;
            public const int FOOD_UPGRADE_ITEM = 2;
            public const int EVOLUTION_WINDOW = 3;
        }

        public static class FeatureCompletedBattleThresholds
        {
            public const int BATTLE_SPEED = 10;
            public const int FOOD_BOOST = 2;
            public const int PAUSE = 1;
            public const int UPGRADES_WINDOW = 1;
            public const int EVOLUTION_WINDOW = 3;
            public const int X2 = 3;
        }

        public sealed class ConfigKeys
        {
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
        }
    }
}