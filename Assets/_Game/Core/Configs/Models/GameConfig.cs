using System;
using System.Collections.Generic;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    public class GameConfig
    {
        public TimelineConfig Timeline;
        public CommonConfig CommonConfig;
        public FoodBoostConfig FoodBoostConfig;
        public List<BattleSpeedConfig> BattleSpeedConfigs;
    }

    public class FoodBoostConfig
    {
        public int Id;
        public int FoodBoostCoefficient;
        public int DailyFoodBoostCount;
        public int RecoverTimeMinutes;
    }

    public class CommonConfig
    {
        public int Id;
        public string FoodIconKey;
        public string CatFoodIconKey;
        public string DogFoodIconKey;
        public string BaseIconKey;
    }
    
    public class BattleSpeedConfig
    {
        public int Id;
        public float SpeedFactor;
        public float Duration;
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
        public string Name;
        public string AgeIconKey;
        public string Description;
        public string DateRange;
        public string BaseKey;
    }

    public class BattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Enemies;
        public string EnvironmentKey;
        public float EnemyBaseHealth;
        public string AmbienceKey;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
        public string BaseKey;
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
        public int InitialFoodAmount;

        public UpgradeItemConfig FoodProduction;
        public UpgradeItemConfig BaseHealth;
    }

    public class UpgradeItemConfig
    {
        public int Id;
        public float Price;
        public Exponential PriceExponential;
        public float Value;
        public float ValueStep;
    }
    
    [Serializable]
    public class WarriorConfig
    {
        public int Id;
        public UnitType Type;
        public float Health;
        public float Speed;
        public WeaponConfig WeaponConfig;
        public string Name;
        public string CatIconKey;
        public string DogIconKey;
        public string DogKey;
        public float Price;
        public string CatKey;
        public int FoodPrice;
        public int CoinsPerKill;
        public float PlayerHealthMultiplier;
        public float EnemyHealthMultiplier;
        public float AttackPerSecond;
        public float AttackDistance;
    }

    [Serializable]
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
        public float PlayerDamageMultiplier;
        public float EnemyDamageMultiplier;
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