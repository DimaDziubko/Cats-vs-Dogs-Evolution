using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._DailyTask
{
    public class DailyTaskStateHandler : IDailyTaskStateHandler
    {
        private readonly IUserContainer _userContainer;

        public DailyTaskStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void CompleteDailyTask()
        {
            _userContainer.State.DailyTasksState.CompleteTask();
            _userContainer.State.TasksState.AddCompletedTask();
        }

        public void ChangeTaskIdx(int id) => 
            _userContainer.State.DailyTasksState.ChangeCurrentTaskIdx(id);

        public void AddProgress(float delta) => 
            _userContainer.State.DailyTasksState.AddProgress(delta);

        public void ClearCompleted() => 
            _userContainer.State.DailyTasksState.ClearCompleted();

        public void ChangeLastTimeGenerated(DateTime newTime) => 
            _userContainer.State.DailyTasksState.ChangeLastTimeGenerated(newTime);
    }
}