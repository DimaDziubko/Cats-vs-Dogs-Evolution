using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class GameConfig
    {
        public TimelineConfig Timeline;
    }

    public class TimelineConfig
    {
        public int Id;
        public List<AgeConfig> Ages;
        public List<BattleConfig> Battles;
        public string BattleAssetKey;
    }

    public class AgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<WarriorConfig> Warriors;
        public string UnitAssetKey;
    }

    public class BattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Enemies;
        public string EnemyAssetKey;
    }

    public class BattleScenario
    {
        public int Id;
        public List<EnemyWave> Waves;
    }
    
    public class EnemyWave
    {
        public int Id;
        public List<EnemySpawnSequence> SpawnSequences;
    }

    public class EnemySpawnSequence
    {
        public int Id;
        public int WarriorIndex;
        public int Amount;
        public float Cooldown;
    }
    

    public class EconomyConfig
    {
        public int Id;
        public float CoinPerBattle;

        public FoodProductionConfig FoodProduction;
        public BaseHealthConfig BaseHealth;
        
        public List<float> WarriorPrices;
    }

    public class BaseHealthConfig
    {
        public int Id;
        public float Price;
        public float PriceFactor;
        public float Health;
        public float HealthFactor;
    }

    public class FoodProductionConfig
    {
        public int Id;
        public float Price;
        public float PriceFactor;
        public float Speed;
        public float SpeedFactor;
    }

    public class WarriorConfig
    {
        public int Id;
        public float Health;
        public float Speed;
        public float Damage;
        public string Name;
    }
}