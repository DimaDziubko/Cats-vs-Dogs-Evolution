using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public enum Task
    {
        None,
    }
    public class DailyTasksState : IDailyTasksStateReadonly
    {
        public float ProgressOnTask;
        public int CurrentTaskIdx;
        public List<Task> CompletedTasks;
        public DateTime LastTimeGenerated;
    }
}