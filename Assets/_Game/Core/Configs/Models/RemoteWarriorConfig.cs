﻿using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Models
{
    public class RemoteWarriorConfig
    {
        public int Id;
        public UnitType Type;
        public float Health;
        public float Speed;
        public RemoteWeaponConfig WeaponConfig;
        public float Price;
        public int FoodPrice;
        public int CoinsPerKill;
        public float PlayerHealthMultiplier;
        public float EnemyHealthMultiplier;
        public float AttackPerSecond;
        public float AttackDistance;
    }
}