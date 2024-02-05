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

            //Timeline
            public const string TIMELINE = "Timeline";
            public const string AGES = "Ages";
            public const string BATTLES = "Battles";
            public const string BATTLE_ASSET_KEY = "BattleAssetKey";

            //Age
            public const string AGE = "Age";
            public const string GEMS_PER_AGE = "GemsPerAge";
            public const string WARRIORS = "Warriors";
            public const string UNIT_ASSET_KEY = "UnitAssetKey";

            //Economy
            public const string ECONOMY = "Economy";
            public const string COINS_PER_BATTLE = "CoinsPerBattle";
            public const string WARRIOR_PRICES = "WarriorPrices";

            //Battle
            public const string BATTLE = "Battle";
            public const string BATTLE_SCENARIO = "BattleScenario";
            public const string ENEMY_ASSET_KEY = "EnemyAssetKey";

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
            
            //BaseHealth
            public const string BASE_HEALTH = "BaseHealth";
            public const string HEALTH_FACTOR = "HealthFactor";

            //Warriors
            public const string WARRIOR = "Warrior";
            public const string DAMAGE = "Damage";
        }
    }
}