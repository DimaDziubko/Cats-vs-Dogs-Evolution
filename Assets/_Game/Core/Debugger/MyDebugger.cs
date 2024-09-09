using System.Collections.Generic;
using _Game.Core._UpgradesChecker;
using _Game.Core.Ads;
using _Game.Core.Data;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Global;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Debugger
{
    public class MyDebugger : MonoBehaviour, IMyDebugger
    {
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> AgeUnitDataPool => GeneralDataPool.AgeStaticData.UnitDataPool.GetData();
         
         [ShowInInspector]
         public Dictionary<int, WeaponData> AgeWeaponDataPool => GeneralDataPool.AgeStaticData.WeaponDataPool.GetData();
         
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_0 => GeneralDataPool.BattleStaticData.UnitDataPools[0].GetData();
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_1 => GeneralDataPool.BattleStaticData.UnitDataPools[1].GetData();
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_2 => GeneralDataPool.BattleStaticData.UnitDataPools[2].GetData();
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_3 => GeneralDataPool.BattleStaticData.UnitDataPools[3].GetData();
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_4 => GeneralDataPool.BattleStaticData.UnitDataPools[4].GetData();
         [ShowInInspector]
         public Dictionary<UnitType, IUnitData> BattleUnitDataPool_5 => GeneralDataPool.BattleStaticData.UnitDataPools[5].GetData();
         
         
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_0 => GeneralDataPool.BattleStaticData.WeaponDataPools[0].GetData();
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_1 => GeneralDataPool.BattleStaticData.WeaponDataPools[1].GetData();
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_2 => GeneralDataPool.BattleStaticData.WeaponDataPools[2].GetData();
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_3 => GeneralDataPool.BattleStaticData.WeaponDataPools[3].GetData();
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_4 => GeneralDataPool.BattleStaticData.WeaponDataPools[4].GetData();
         [ShowInInspector]
         public Dictionary<int, WeaponData> BattleWeaponDataPool_5 => GeneralDataPool.BattleStaticData.WeaponDataPools[5].GetData();
         //
         // [ShowInInspector] 
         // public Dictionary<Race, Sprite> FoodIcon => GeneralDataPool.AgeStaticData.FoodIcons.GetData();
         //
         // [ShowInInspector] 
         // public Dictionary<int, ShopItemStaticData> ShopItemStaticData => GeneralDataPool.ShopItemStaticDataPool.GetData();
         //
         // [ShowInInspector]
         // public float PlayerTowerHealth => GeneralDataPool.AgeDynamicData.UpgradeItems.BaseHealth.Amount;
         //
         // [ShowInInspector]
         // public float FoodProductionSpeed => GeneralDataPool.AgeDynamicData.UpgradeItems.FoodProductionSpeed.Amount;
         //
         // public GeneralDataPool GeneralDataPool { get; set; }
         //
         // [ShowInInspector, ReadOnly]
         // public Dictionary<GameScreen, NotificationData> NotificationData { get; set; }
         //
         // [ShowInInspector]
         // public BattleState State => BattleManager.State;
         //
         // [ShowInInspector]
         // public bool IsPaused => BattleManager.IsPaused;
         //
         // [ShowInInspector] 
         // public TimelineConfig TimelineConfig => UserContainer.GameConfig.CurrentTimeline;

         // [ShowInInspector, ReadOnly]
         // public Dictionary<int, EnvironmentData> EnvironmentData =>
         //     GeneralDataPool.BattleStaticData.EnvironmentPool;
         //
         // [ShowInInspector, ReadOnly]
         // public int DailyFoodBoost =>
         //     UserContainer.State.FoodBoost.DailyFoodBoostCount;
         //
         // [ShowInInspector, ReadOnly]
         // public DateTime LastDailyFoodBoost =>
         //     UserContainer.State.FoodBoost.LastDailyFoodBoost;
         //
         //
         // [ShowInInspector, ReadOnly]
         // public SpeedBoostBtnModel SpeedBoostBtnModel => SpeedBoostService.SpeedBoostBtnModel;
         //
         // [ShowInInspector, ReadOnly]
         // public BattleSpeedBtnModel BattleSpeedBtnModel => BattleSpeedService.BattleSpeedBtnModel;
         //
         // [ShowInInspector, ReadOnly] 
         // public bool IsTimeForInterstitial => CasAdsService.IsTimeForInterstitial;
         //
         // [ShowInInspector, ReadOnly] 
         // public float TimeLeft => CasAdsService.TimeLeft;
         //
         // [ShowInInspector, ReadOnly] 
         // public bool CanShowInterstitial => CasAdsService.CanShowInterstitial;
         public GeneralDataPool GeneralDataPool { get; set; }
         public Dictionary<GameScreen, NotificationData> NotificationData { get; set; }
         public UserContainer UserContainer { get; set; }
         public BattleSpeedService BattleSpeedService { get; set; }
         public SpeedBoostService SpeedBoostService { get; set; }
         public CasAdsService CasAdsService { get; set; }
         public BattleManager BattleManager { get; set; }
         public UINotifier UINotifier { get; set; }
    }
    

    public interface IMyDebugger
    {
         GeneralDataPool GeneralDataPool { get; set; }
         Dictionary<GameScreen, NotificationData> NotificationData { get; set; }
         UserContainer UserContainer { get; set; }
         BattleSpeedService BattleSpeedService { get; set; }
         SpeedBoostService SpeedBoostService { get; set; }
         CasAdsService CasAdsService { get; set; }
         BattleManager BattleManager { get; set; }
         UINotifier UINotifier { get; set; }
    }
}