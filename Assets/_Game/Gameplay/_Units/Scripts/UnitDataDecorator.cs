using _Game.Gameplay._Weapon.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitDataDecorator : IUnitData
    {
        protected readonly IUnitData _unitData;
        protected UnitDataDecorator(IUnitData unitData) => 
            _unitData = unitData;

        public virtual Unit Prefab => _unitData.Prefab;
        public virtual int UnitLayer => _unitData.UnitLayer;
        public virtual int AggroLayer => _unitData.AggroLayer;
        public virtual int AttackLayer => _unitData.AttackLayer;
        public virtual Race Race => _unitData.Race;
        public WeaponType WeaponType => _unitData.WeaponType;
        public int CoinsPerKill => _unitData.CoinsPerKill;
        public float AttackDistance => _unitData.AttackDistance;
        public float Speed => _unitData.Speed;
        public float AttackPerSecond => _unitData.AttackPerSecond;
        public UnitType Type => _unitData.Type;
        public virtual float Damage => _unitData.Damage;
        public float SplashRadius => _unitData.SplashRadius;
        public int WeaponId => _unitData.WeaponId;
        public Sprite Icon => _unitData.Icon;
        public string Name => _unitData.Name;
        public string CatKey => _unitData.CatKey;
        public string DogKey => _unitData.DogKey;
        public float Price => _unitData.Price;
        public int FoodPrice => _unitData.FoodPrice;

        public virtual float GetUnitHealthForFaction(Faction faction) => 
            _unitData.GetUnitHealthForFaction(faction);

        public virtual float GetStatBoost(StatType statType) => 
            _unitData.GetStatBoost(statType);
    }
}