using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using Assets._Game.Core.Communication;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo110 : StateMigrationBase
    {
        public override string TargetVersion => "1.1.0";

        public override void Migrate(ref UserAccountState state)
        {
            state.PurchaseDataState ??= new PurchaseDataState()
            {
                BoudhtIAPs = new List<BoughtIAP>()
            };

            state.FreeGemsPackState ??= new FreeGemsPackState()
            {
                FreeGemPackCount = 2,
                LastFreeGemPackDay = DateTime.UtcNow
            };
            
            state.DailyTasksState ??= new DailyTasksState()
            {
                ProgressOnTask = 0,
                CompletedTasks = new List<Task>(),
                CurrentTaskIdx = 0,
                LastTimeGenerated = DateTime.Now
            };
            
            state.TasksState ??= new TasksState()
            {
                TotalCompletedTasks = 0
            };
        }
    }
}