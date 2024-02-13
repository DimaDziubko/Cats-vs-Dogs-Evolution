using System;
using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;
using UnityEngine;

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

        //TODO Change save system
        public event Action<UnitType> OpenedUnit;
        public event Action NextAgeOpened;
        public event Action NextTimelineOpened;
        public event Action FoodProductionLevelChanged;
        public event Action BaseHealthLevelChanged;
        
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
        
        public void OpenNextAge(int ageId)
        {
            AgeId = ageId;
            FoodProductionLevel = 0;
            BaseHealthLevel = 0;
            AllBattlesWon = false;
            MaxBattle = 0;
            OpenUnits = new List<UnitType>() {UnitType.Light};
            NextAgeOpened?.Invoke();
        }
        
        public void OpenNextTimeline(int timelineId)
        {
            TimelineId = timelineId;
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
            FoodProductionLevelChanged?.Invoke();
        }
        
        public void ChangeBaseHealthLevel()
        {
            BaseHealthLevel++;
            BaseHealthLevelChanged?.Invoke();
        }
    }
}