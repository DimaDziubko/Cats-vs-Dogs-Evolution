﻿using System.Collections.Generic;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Common.Scripts;
using _Game.UI._Environment;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Pin.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Debugger
{
    public class MyDebugger : MonoBehaviour, IMyDebugger
    {
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> AgeUnitDataPool => GeneralDataPool.AgeStaticData.UnitDataPool.GetData();

        [ShowInInspector]
        public Dictionary<UnitType, UnitBuilderBtnStaticData> UnitBuilderDataPool => GeneralDataPool.AgeStaticData.UnitBuilderDataPool.GetData();
        
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> AgeWeaponDataPool => GeneralDataPool.AgeStaticData.WeaponDataPool.GetData();
        
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_0 => GeneralDataPool.BattleStaticData.UnitDataPools[0].GetData();
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_1 => GeneralDataPool.BattleStaticData.UnitDataPools[1].GetData();
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_2 => GeneralDataPool.BattleStaticData.UnitDataPools[2].GetData();
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_3 => GeneralDataPool.BattleStaticData.UnitDataPools[3].GetData();
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_4 => GeneralDataPool.BattleStaticData.UnitDataPools[4].GetData();
        [ShowInInspector]
        public Dictionary<UnitType, UnitData> BattleUnitDataPool_5 => GeneralDataPool.BattleStaticData.UnitDataPools[5].GetData();
        
        
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_0 => GeneralDataPool.BattleStaticData.WeaponDataPools[0].GetData();
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_1 => GeneralDataPool.BattleStaticData.WeaponDataPools[1].GetData();
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_2 => GeneralDataPool.BattleStaticData.WeaponDataPools[2].GetData();
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_3 => GeneralDataPool.BattleStaticData.WeaponDataPools[3].GetData();
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_4 => GeneralDataPool.BattleStaticData.WeaponDataPools[4].GetData();
        [ShowInInspector]
        public Dictionary<WeaponType, WeaponData> BattleWeaponDataPool_5 => GeneralDataPool.BattleStaticData.WeaponDataPools[5].GetData();

        [ShowInInspector] 
        public Dictionary<Race, Sprite> FoodIcon => GeneralDataPool.AgeStaticData.FoodIcons.GetData();
        
        [ShowInInspector]
        public float PlayerTowerHealth => GeneralDataPool.AgeDynamicData.UpgradeItems.BaseHealth.Amount;
        
        [ShowInInspector]
        public float FoodProductionSpeed => GeneralDataPool.AgeDynamicData.UpgradeItems.FoodProductionSpeed.Amount;
        
        public GeneralDataPool GeneralDataPool { get; set; }
        
        [ShowInInspector, ReadOnly]
        public Dictionary<Window, NotificationData> NotificationData { get; set; }

        [ShowInInspector, ReadOnly]
        public Dictionary<int, EnvironmentData> EnvironmentData =>
            GeneralDataPool.BattleStaticData.EnvironmentPool;
    }
    

    public interface IMyDebugger
    {
        GeneralDataPool GeneralDataPool { get; set; }
        Dictionary<Window, NotificationData> NotificationData { get; set; }
    }
}