using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    [Serializable]
    public class UnitData : IUnitData
    {
        public Unit Prefab { get; set; }
        public int UnitLayer { get; set; }
        public int AggroLayer { get; set; }
        public int AttackLayer { get; set; }
        public Race Race { get; set; }
        public Sprite Icon { get; set; }
        public Sprite EnemyIcon { get; set; }
        
        private WarriorConfig _config;
        
        public UnitData(WarriorConfig config)
        {
            _config = config;
        }

        public WeaponType WeaponType => _config.WeaponConfig.WeaponType;
        public float CoinsPerKill => _config.CoinsPerKill;
        public float AttackDistance => _config.AttackDistance;
        public float Speed => _config.Speed;
        public float AttackPerSecond => _config.AttackPerSecond;
        public UnitType Type => _config.Type;
        public float Damage => _config.WeaponConfig.Damage;
        public float SplashRadius => _config.WeaponConfig.SplashRadius;
        public int WeaponId => _config.WeaponConfig.Id;
        public string CatKey => _config.CatKey;
        public string DogKey => _config.DogKey;
        public float Price => _config.Price;
        public int FoodPrice => _config.FoodPrice;
        public string Name => _config.Name;

        public float GetUnitHealthForFaction(Faction faction) => 
            _config.GetUnitHealthForFaction(faction);

        public float GetStatBoost(StatType statType)
        {
            float minBoostValue = 1;

            switch (statType)
            {
                case StatType.Damage:
                    return minBoostValue;
                case StatType.Health:
                    return minBoostValue;
                default:
                    return minBoostValue;
            }
        }
    }
}