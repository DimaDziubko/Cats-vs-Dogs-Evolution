using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public class DailyTasksState : IDailyTasksStateReadonly
    {
        public float ProgressOnTask;
        public int CurrentTaskIdx;
        public List<int> CompletedTasks;
        public DateTime LastTimeGenerated;

        public event Action ProgressChanged;
        public event Action TaskCompletedChanged;

        List<int> IDailyTasksStateReadonly.CompletedTasks => CompletedTasks;
        DateTime IDailyTasksStateReadonly.LastTimeGenerated => LastTimeGenerated;
        int IDailyTasksStateReadonly.CurrentTaskIdx => CurrentTaskIdx;
        float IDailyTasksStateReadonly.ProgressOnTask => ProgressOnTask;

        public void ChangeLastTimeGenerated(DateTime time)
        {
            LastTimeGenerated = time;
        }

        public void ChangeCurrentTaskIdx(int newIdx)
        {
            CurrentTaskIdx = newIdx;
        }
        
        public void CompleteTask()
        {
            CompletedTasks.Add(CurrentTaskIdx);
            AddProgress(-ProgressOnTask);
            TaskCompletedChanged?.Invoke();
        }

        public void AddProgress(float delta)
        {
            ProgressOnTask += delta;
            ProgressChanged?.Invoke();
        }

        public void ClearCompleted()
        {
            CompletedTasks.Clear();
        }
    }
}