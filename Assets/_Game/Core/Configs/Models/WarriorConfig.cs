using System;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models
{
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
        public AssetReference CatIconAtlas;
        public AssetReference DogIconAtlas;
        public string CatIconKey;
        public string DogIconKey;
        public string CatIconName;
        public string DogIconName;
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