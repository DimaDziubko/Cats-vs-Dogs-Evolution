using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;

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
    }

    public class AgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<WarriorConfig> Warriors;
        public string FoodIconKey;
        public string PlayerBaseKey;
    }

    public class BattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Enemies;
        public string BackgroundKey;
        public float EnemyBaseHealth;
        public string EnemyBaseKey;
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
        public UnitType Type;
        public int Amount;
        public float Cooldown;
    }
    

    public class EconomyConfig
    {
        public int Id;
        public float CoinPerBattle;

        public FoodProductionConfig FoodProduction;
        public BaseHealthConfig BaseHealth;
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
        public int InitialFoodAmount;
    }

    public class WarriorConfig
    {
        public int Id;
        public float Health;
        public float Speed;
        public float Damage;
        public string Name;
        public string IconKey;
        public string EnemyKey;
        public float Price;
        public string PlayerKey;
        public int FoodPrice;
        public UnitType Type;
    }
}