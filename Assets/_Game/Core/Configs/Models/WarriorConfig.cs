using System;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Models
{
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
}