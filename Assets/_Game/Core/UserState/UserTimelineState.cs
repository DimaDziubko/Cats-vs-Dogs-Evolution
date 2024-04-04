using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState
{
    public class UserTimelineState : IUserTimelineStateReadonly
    {
        public int TimelineId;
        public int MaxBattle;
        public bool AllBattlesWon;

        public int AgeId;
        public int FoodProductionLevel;
        public int BaseHealthLevel;
        public List<UnitType> OpenUnits;
        
        public event Action<UnitType> OpenedUnit;
        public event Action NextAgeOpened;
        public event Action NextBattleOpened;
        public event Action NextTimelineOpened;
        public event Action<UpgradeItemType> UpgradeItemChanged;
        public event Action LastBattleWon;
        
        int IUserTimelineStateReadonly.TimelineId => TimelineId;
        
        int IUserTimelineStateReadonly.AgeId => AgeId;
        int IUserTimelineStateReadonly.MaxBattle => MaxBattle;
        
        bool IUserTimelineStateReadonly.AllBattlesWon => AllBattlesWon;

        int IUserTimelineStateReadonly.FoodProductionLevel => FoodProductionLevel;
        int IUserTimelineStateReadonly.BaseHealthLevel => BaseHealthLevel;
        
        List<UnitType> IUserTimelineStateReadonly.OpenUnits => OpenUnits;
        
        public void OpenUnit(in UnitType type)
        {
            OpenUnits.Add(type);
            OpenedUnit?.Invoke(type);
        }
        
        public void OpenNextAge()
        {
            AgeId++;
            FoodProductionLevel = 0;
            BaseHealthLevel = 0;
            AllBattlesWon = false;
            MaxBattle = 0;
            OpenUnits = new List<UnitType>() {UnitType.Light};
            
            NextAgeOpened?.Invoke();
        }
        
        public void OpenNextTimeline()
        {
            TimelineId++;
            AgeId = 0;
            FoodProductionLevel = 0;
            BaseHealthLevel = 0;
            AllBattlesWon = false;
            MaxBattle = 0;
            OpenUnits = new List<UnitType>() {UnitType.Light};
            
            NextTimelineOpened?.Invoke();
        }

        public void ChangeFoodProductionLevel()
        {
            FoodProductionLevel++;
            UpgradeItemChanged?.Invoke(UpgradeItemType.FoodProduction);
        }
        
        public void ChangeBaseHealthLevel()
        {
            BaseHealthLevel++;
            UpgradeItemChanged?.Invoke(UpgradeItemType.BaseHealth);
        }

        public void OpenNextBattle(int nextBattle)
        {
            MaxBattle = nextBattle;
            NextBattleOpened?.Invoke();
        }

        public void SetAllBattlesWon(bool allBattlesWon)
        {
            AllBattlesWon = allBattlesWon;
            LastBattleWon?.Invoke();
        }
    }
}