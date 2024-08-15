using System;
using System.Collections.Generic;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    
    [CreateAssetMenu(fileName = "GeneralWarriorConfig", menuName = "Configs/Warriors")]
    [Serializable]
    public class GeneralWarriorConfig : ScriptableObject
    {
        public List<WarriorConfig> BattleConfigs;
    }
    
    [CreateAssetMenu(fileName = "WarriorConfig", menuName = "Configs/Warrior")]
    [Serializable]
    public class WarriorConfig : ScriptableObject
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
    
    public class RemoteWarriorConfig
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
}