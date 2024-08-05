using System.Collections.Generic;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.Core.Data.Age.Static
{
    public class AgeStaticData
    {
        public DataPool<UnitType, UnitData> UnitDataPool { get; set; }
        public DataPool<int, WeaponData> WeaponDataPool { get; set; }
        public DataPool<UnitType, UnitBuilderBtnStaticData> UnitBuilderDataPool { get; set; }
        public DataPool<UnitType, UnitUpgradeItemStaticData> UnitUpgradesPool { get; set; }
        public BaseStaticData BaseStaticData { get; set; }
        public DataPool<Race, Sprite> FoodIcons { get; set; }
        public Sprite TowerHealthIcon { get; set; }
        
        
        public UnitData ForUnit(UnitType type) => UnitDataPool.ForType(type);
        public Dictionary<UnitType, UnitBuilderBtnStaticData> GetBuilderButtonsData() => UnitBuilderDataPool.GetData();
        public UnitBuilderBtnStaticData GetBuilderButtonData(UnitType type) => UnitBuilderDataPool.ForType(type);
        public Dictionary<UnitType, UnitUpgradeItemStaticData> GetUnitUpgradeItems() => UnitUpgradesPool.GetData();
        public WeaponData ForWeapon(int weaponId) => WeaponDataPool.ForType(weaponId);
        public BaseStaticData ForBase() => BaseStaticData;
        public Sprite ForFoodIcon(Race currentRace) => FoodIcons.ForType(currentRace);
        public Sprite ForTowerIcon() => TowerHealthIcon;

        public void Cleanup()
        {
            UnitDataPool.Cleanup();
            WeaponDataPool.Cleanup();
            UnitBuilderDataPool.Cleanup();
            UnitUpgradesPool.Cleanup();
            BaseStaticData = null;
            FoodIcons.Cleanup();
            TowerHealthIcon = null;
        }

    }
}