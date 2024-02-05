using System;
using System.Collections.Generic;

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
        public List<int> OpenUnits;

        public event Action Changed;
        
        int IUserTimelineStateReadonly.TimelineId => TimelineId;
        
        int IUserTimelineStateReadonly.AgeId => AgeId;
        int IUserTimelineStateReadonly.MaxBattle => MaxBattle;
        
        bool IUserTimelineStateReadonly.AllBattlesWon => AllBattlesWon;

        int IUserTimelineStateReadonly.FoodProductionLevel => FoodProductionLevel;
        int IUserTimelineStateReadonly.BaseHealthLevel => BaseHealthLevel;
        
        List<int> IUserTimelineStateReadonly.OpenUnits => OpenUnits;
        
        public void OpenUnit(in int unitIndex)
        {
            OpenUnits.Add(unitIndex);
            Changed?.Invoke();
        }
    }
}