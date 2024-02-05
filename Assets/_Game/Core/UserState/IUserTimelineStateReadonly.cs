using System;
using System.Collections.Generic;

namespace _Game.Core.UserState
{
    public interface IUserTimelineStateReadonly
    {
        int TimelineId { get; }
        int AgeId { get; }
        int MaxBattle { get; }
        bool AllBattlesWon { get; }
        int FoodProductionLevel { get; }
        int BaseHealthLevel { get; }

        List<int> OpenUnits { get; }

        event Action Changed;
    }
}