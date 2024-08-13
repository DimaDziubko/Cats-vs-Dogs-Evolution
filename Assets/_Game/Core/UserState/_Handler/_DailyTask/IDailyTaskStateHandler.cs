using System;

namespace _Game.Core.UserState._Handler._DailyTask
{
    public interface IDailyTaskStateHandler
    {
        void CompleteDailyTask();
        void ChangeTaskIdx(int configId);
        void AddProgress(float delta);
        void ClearCompleted();
        void ChangeLastTimeGenerated(DateTime utcNow);
    }
}