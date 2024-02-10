using System;
using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;

namespace _Game.Core.UserState
{
    public class UserTimelineState : IUserTimelineStateReadonly
    {
        public int TimelineId;

        public int AgeId;
        public int MaxBattle;
        
        public bool AllBattlesWon;

        public int FoodProductionLevel;
        public int BaseHealthLevel;

        //TODO Change save system
        public List<UnitType> OpenUnits;

        public event Action Changed;
        
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
            Changed?.Invoke();
        }
    }
}