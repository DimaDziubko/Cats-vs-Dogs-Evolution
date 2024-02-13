namespace _Game.Utils
{
    public sealed class Constants
    {
        public sealed class Scenes
        {
            public const string STARTUP = "Startup";
            public const string BATTLE_MODE = "BattleMode";
        }
        
        public sealed class ConfigKeys
        {
            //Common
            public const string ID = "Id";
            public const string PRICE = "Price";
            public const string PRICE_FACTOR = "PriceFactor";
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
            public const string FOOD_ICON_KEY = "FoodIconKey";
            public const string PLAYER_BASE_KEY = "PlayerBaseKey";

            //Economy
            public const string ECONOMY = "Economy";
            public const string COINS_PER_BATTLE = "CoinsPerBattle";

            //Battle
            public const string BATTLE = "Battle";
            public const string BATTLE_SCENARIO = "BattleScenario";
            public const string BACKGROUND_KEY = "BackgroundKey";
            public const string ENEMY_BASE_HEALTH = "EnemyBaseHealth";
            public const string ENEMY_BASE_KEY = "EnemyBaseKey";

            //BattleScenario
            public const string WAVES = "Waves";

            //Wave
            public const string WAVE = "Wave";
            public const string ENEMY_SPAWN_SEQUENCES = "EnemySpawnSequences";

            //SpawnSequence
            public const string ENEMY_SPAWN_SEQUENCE = "EnemySpawnSequence";
            public const string AMOUNT = "Amount";
            public const string COOLDOWN = "Cooldown";

            //Food production
            public const string FOOD_PRODUCTION = "FoodProduction";
            public const string SPEED_FACTOR = "SpeedFactor";
            public const string INITIAL_FOOD_AMOUNT = "InitialFoodAmount";

            //BaseHealth
            public const string BASE_HEALTH = "BaseHealth";
            public const string HEALTH_FACTOR = "HealthFactor";

            //Warriors
            public const string WARRIOR = "Warrior";
            public const string DAMAGE = "Damage";
            public const string ICON_KEY = "IconKey";
            public const string ENEMY_KEY = "EnemyKey";
            public const string PLAYER_KEY = "PlayerKey";
            public const string FOOD_PRICE = "FoodPrice";
        }
    }
}