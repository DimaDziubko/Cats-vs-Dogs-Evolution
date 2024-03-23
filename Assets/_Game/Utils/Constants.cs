namespace _Game.Utils
{
    public sealed class Constants
    {
        public sealed class Scenes
        {
            public const string STARTUP = "Startup";
            public const string BATTLE_MODE = "BattleMode";
        }
        
        public sealed class SortingLayer
        {
            public const float SORTING_MAX_Y = 5f;
            public const float SORTING_MIN_Y = -5f;
            public const float SORTING_TRESHOLD = 0.1f;
            public const int SORTING_ORDER_MIN = -32768;
            public const int SORTING_ORDER_MAX = 32767;
        }
        
        public sealed class Layer
        {
            public const int PLAYER_PROJECTILE = 16;
            public const int ENEMY_PROJECTILE = 17;
            public const int PLAYER = 8;
            public const int ENEMY = 9;
            public const int ENEMY_BASE = 14;
            public const int PLAYER_BASE = 15;
        }
        
        public sealed class ConfigKeys
        {
            //Common
            public const string COMMON = "Common";
            public const string FOOD_ICON_KEY = "FoodIconKey";
            public const string BASE_ICON_KEY = "BaseIconKey";
            public const string MISSING_KEY = "-";

            //Timeline common
            public const string ID = "Id";
            public const string PRICE = "Price";
            public const string PRICE_EXPONENTIAL = "PriceExponential";
            public const string SPEED = "Speed";
            public const string HEALTH = "Health";
            public const string NAME = "Name";
            public const string UNIT_TYPE = "UnitType";

            //Timeline
            public const string TIMELINE = "Timeline";
            public const string AGES = "Ages";
            public const string BATTLES = "Battles";

            //Age
            public const string AGE = "Age";
            public const string GEMS_PER_AGE = "GemsPerAge";
            public const string WARRIORS = "Warriors";
            public const string PLAYER_BASE_KEY = "PlayerBaseKey";
            public const string AGE_ICON_KEY = "AgeIconKey";

            //Economy
            public const string ECONOMY = "Economy";
            public const string COINS_PER_BATTLE = "CoinsPerBattle";

            //Battle
            public const string BATTLE = "Battle";
            public const string BATTLE_SCENARIO = "BattleScenario";
            public const string BACKGROUND_KEY = "BackgroundKey";
            public const string ENEMY_BASE_HEALTH = "EnemyBaseHealth";
            public const string ENEMY_BASE_KEY = "EnemyBaseKey";
            public const string BGM_KEY = "BGMKey";

            //BattleScenario
            public const string WAVES = "Waves";

            //Wave
            public const string WAVE = "Wave";
            public const string ENEMY_SPAWN_SEQUENCES = "EnemySpawnSequences";

            //SpawnSequence
            public const string ENEMY_SPAWN_SEQUENCE = "EnemySpawnSequence";
            public const string AMOUNT = "Amount";
            public const string COOLDOWN = "Cooldown";
            public const string SEQUENCE_START_DELAY = "SequenceStartDelay";

            //Food production
            public const string FOOD_PRODUCTION = "FoodProduction";
            public const string SPEED_STEP = "SpeedStep";
            public const string INITIAL_FOOD_AMOUNT = "InitialFoodAmount";

            //Base
            public const string BASE_HEALTH = "BaseHealth";
            public const string HEALTH_STEP = "HealthStep";
            public const string COINS_PER_BASE = "CoinsPerBase";

            //Warriors
            public const string WARRIOR = "Warrior";
            public const string ICON_KEY = "IconKey";
            public const string ENEMY_KEY = "EnemyKey";
            public const string PLAYER_KEY = "PlayerKey";
            public const string FOOD_PRICE = "FoodPrice";
            public const string COINS_PER_KILL = "CoinsPerKill";

            //Weapon
            public const string WEAPON = "Weapon";
            public const string DAMAGE = "Damage";
            public const string WEAPON_TYPE = "WeaponType";
            public const string PROJECTILE_SPEED = "ProjectileSpeed";
            public const string PROJECTILE_KEY = "ProjectileKey";
            public const string TRAJECTORY_WARP_FACTOR = "TrajectoryWarpFactor";
            public const string PROJECTILE_EXPLOSION_KEY = "ProjectileExplosionKey";
            public const string MUZZLE_KEY = "MuzzleKey";
            public const string SPLASH_RADIUS = "SplashRadius" ;

            //Polynomial
            public const string EXPONENTIAL = "Exponential";
            public const string COEFFICIENT_A = "CoefficientA";
            public const string COEFFICIENT_B = "CoefficientB";
            public const string COEFFICIENT_C = "CoefficientC";
        }
    }
}