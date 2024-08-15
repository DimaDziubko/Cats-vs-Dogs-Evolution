using System;
using System.Collections.Generic;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.UserState._State
{
    public class TimelineState : ITimelineStateReadonly
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
        public event Action<UpgradeItemType, int> UpgradeItemLevelChanged;
        public event Action LastBattleWon;

        int ITimelineStateReadonly.TimelineId => TimelineId;

        int ITimelineStateReadonly.AgeId => AgeId;
        int ITimelineStateReadonly.MaxBattle => MaxBattle;

        bool ITimelineStateReadonly.AllBattlesWon => AllBattlesWon;

        int ITimelineStateReadonly.FoodProductionLevel => FoodProductionLevel;
        int ITimelineStateReadonly.BaseHealthLevel => BaseHealthLevel;

        List<UnitType> ITimelineStateReadonly.OpenUnits => OpenUnits;

        public void OpenUnit(in UnitType type)
        {
            OpenUnits.Add(type);
            OpenedUnit?.Invoke(type);
        }

        public void OpenNewAge(bool isNext = true)
        {
            if (isNext)
            {
                AgeId++;
            }
            else
            {
                AgeId--;
            }

            FoodProductionLevel = 0;
            BaseHealthLevel = 0;
            AllBattlesWon = false;
            MaxBattle = 0;
            OpenUnits = new List<UnitType>() { UnitType.Light };

            NextAgeOpened?.Invoke();
        }

        public void OpenNewTimeline(bool isNext = true)
        {
            if (isNext)
            {
                TimelineId++;
            }
            else
            {
                TimelineId--;
            }

            AgeId = 0;
            FoodProductionLevel = 0;
            BaseHealthLevel = 0;
            AllBattlesWon = false;
            MaxBattle = 0;
            OpenUnits = new List<UnitType>() { UnitType.Light };

            NextTimelineOpened?.Invoke();
        }

        public void ChangeFoodProductionLevel()
        {
            FoodProductionLevel++;
            UpgradeItemLevelChanged?.Invoke(UpgradeItemType.FoodProduction, FoodProductionLevel);
        }

        public void ChangeBaseHealthLevel()
        {
            BaseHealthLevel++;
            UpgradeItemLevelChanged?.Invoke(UpgradeItemType.BaseHealth, BaseHealthLevel);
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