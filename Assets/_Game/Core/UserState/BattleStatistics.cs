using System;

namespace Assets._Game.Core.UserState
{
    public class BattleStatistics : IBattleStatisticsReadonly
    {
        public int BattlesCompleted;

        public event Action CompletedBattlesCountChanged;

        int IBattleStatisticsReadonly.BattlesCompleted => BattlesCompleted;

        public void AddCompletedBattle()
        {
            BattlesCompleted++;    
            CompletedBattlesCountChanged?.Invoke();
        }
    }

    public interface IBattleStatisticsReadonly
    {
        int BattlesCompleted { get; }
        event Action CompletedBattlesCountChanged;
    }
}