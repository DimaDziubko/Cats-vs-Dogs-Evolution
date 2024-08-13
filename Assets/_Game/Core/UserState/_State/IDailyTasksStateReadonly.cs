using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IDailyTasksStateReadonly
    {
        event Action ProgressChanged;
        event Action TaskCompletedChanged;
        
        List<int> CompletedTasks { get; }
        DateTime LastTimeGenerated { get; }
        int CurrentTaskIdx { get; }
        float ProgressOnTask { get; }
    }
}