using System;

namespace _Game.Core.UserState
{
    public class BattleStatistics : IBattleStatisticsReadonly
    {
        public int BattlesCompleted;

        public event Action<int> CompletedBattlesCountChanged;

        int IBattleStatisticsReadonly.BattlesCompleted => BattlesCompleted;

        public void AddCompletedBattle()
        {
            BattlesCompleted++;    
            CompletedBattlesCountChanged?.Invoke(BattlesCompleted);
        }
    }

    public interface IBattleStatisticsReadonly
    {
        int BattlesCompleted { get; }
        event Action<int> CompletedBattlesCountChanged;
    }
}