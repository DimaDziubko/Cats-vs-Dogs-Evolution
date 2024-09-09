using _Game.Gameplay._Weapon.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public interface IUnitData
    {
        Unit Prefab { get;}
        int UnitLayer { get;}
        int AggroLayer { get;}
        int AttackLayer { get;}
        Race Race { get;}
        WeaponType WeaponType { get; }
        int CoinsPerKill { get;}
        float AttackDistance { get;}
        float Speed { get;}
        float AttackPerSecond { get;}
        UnitType Type { get;}
        float Damage { get;}
        float SplashRadius { get;}
        int WeaponId { get;}
        public Sprite Icon { get;}
        string Name { get;}
        string CatKey { get;}
        string DogKey { get;}
        float Price { get;}
        int FoodPrice { get;}
        float GetUnitHealthForFaction(Faction faction);
        float GetStatBoost(StatType statType);
    }
}