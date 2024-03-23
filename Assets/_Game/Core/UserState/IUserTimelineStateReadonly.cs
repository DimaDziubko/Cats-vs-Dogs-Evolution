using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

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
        event Action NextBattleOpened;
        event Action LastBattleWon;
        event Action<UpgradeItemType> UpgradeItemChanged;
    }
}