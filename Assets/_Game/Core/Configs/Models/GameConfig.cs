using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    public class GameConfig
    {
        public TimelineConfig Timeline;
        public CommonConfig CommonConfig;
    }

    public class CommonConfig
    {
        public int Id;
        public string FoodIconKey;
        public string BaseIconKey;
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
        public string PlayerBaseKey;
        public string Name;
        public string AgeIconKey;
    }

    public class BattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Enemies;
        public string EnvironmentKey;
        public float EnemyBaseHealth;
        public string EnemyBaseKey;
        public string BGMKey;
        public float CoinsPerBase;
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
        public float StartDelay;
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
        public Exponential PriceExponential;
        public float Health;
        public float HealthStep;
    }

    public class FoodProductionConfig
    {
        public int Id;
        public float Price;
        public Exponential PriceExponential;
        public float Speed;
        public float SpeedStep;
        public int InitialFoodAmount;
    }

    public class WarriorConfig
    {
        public int Id;
        public UnitType Type;
        public float Health;
        public float Speed;
        public WeaponConfig WeaponConfig;
        public string Name;
        public string IconKey;
        public string EnemyKey;
        public float Price;
        public string PlayerKey;
        public int FoodPrice;
        public int CoinsPerKill;
    }

    public class WeaponConfig
    {
        public int Id;
        public WeaponType WeaponType;
        public float Damage;
        public float ProjectileSpeed;
        public string ProjectileKey;
        public float TrajectoryWarpFactor;
        public string MuzzleKey;
        public string ProjectileExplosionKey;
        public float SplashRadius;
    }

    public class Exponential
    {
        public int Id;
        public float A;
        public float B;
        public float C;

        public float GetValue(int level)
        {
            float cost = A * Mathf.Exp(B * level) + C;
            return cost;
        }
    }
}