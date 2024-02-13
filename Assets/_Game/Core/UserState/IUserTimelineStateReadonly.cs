using System;
using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;

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
        List<UnitType> OpenUnits { get; }
        event Action<UnitType> OpenedUnit;
        event Action NextAgeOpened;
        event Action NextTimelineOpened;
        event Action FoodProductionLevelChanged;
        event Action BaseHealthLevelChanged;
    }
}