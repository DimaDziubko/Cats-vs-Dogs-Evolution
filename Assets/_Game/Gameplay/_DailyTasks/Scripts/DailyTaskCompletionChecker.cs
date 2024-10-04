using System;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Units.Scripts;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public interface IDailyTaskCompletionChecker
    {
        event Action<DailyTask> DailyTaskUpdated;
        DailyTask CurrentTask { get;}
        void OnUnitSpawned(Faction faction, UnitType type);
        void OnUnitDead(Faction faction, UnitType type);
    }

    public class DailyTaskCompletionChecker : 
        IDailyTaskCompletionChecker, 
        IInitializable, 
        IDisposable, 
        IFoodListener
    {
        public event Action<DailyTask> DailyTaskUpdated;

        private readonly IUserContainer _userContainer;
        private readonly IDailyTaskGenerator _taskGenerator;
        private readonly ICoinCounter _coinCounter;
        private readonly IMyLogger _logger;

        private IDailyTasksStateReadonly DailyState => _userContainer.State.DailyTasksState;
        private IAdsWeeklyWatchStateReadonly AdsWeeklyWatchState => _userContainer.State.AdsWeeklyWatchState;

        public DailyTask CurrentTask => _taskGenerator.CurrentTask;

        public DailyTaskCompletionChecker(
            IUserContainer userContainer,
            IDailyTaskGenerator taskGenerator,
            ICoinCounter coinCounter,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _taskGenerator = taskGenerator;
            _coinCounter = coinCounter;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            _taskGenerator.DailyTaskGenerated += OnTaskGenerated;
            OnTaskGenerated(_taskGenerator.CurrentTask);
            
            AdsWeeklyWatchState.AdsWatchedChanged += OnAdsWatchedChanged;
            _coinCounter.CoinsCollected += OnCoinCollected;

            DailyState.ProgressChanged += OnProgressChanged;
        }

        void IDisposable.Dispose()
        {
            AdsWeeklyWatchState.AdsWatchedChanged -= OnAdsWatchedChanged;
            _taskGenerator.DailyTaskGenerated -= OnTaskGenerated;
            _coinCounter.CoinsCollected -= OnCoinCollected;

            DailyState.ProgressChanged -= OnProgressChanged;
        }

        void IDailyTaskCompletionChecker.OnUnitSpawned(Faction faction, UnitType type)
        {
            switch (faction)
            {
                case Faction.Player:
                    CheckForSpawnTasks(type);
                    break;
                case Faction.Enemy:
                    break;
            }
        }

        void IDailyTaskCompletionChecker.OnUnitDead(Faction faction, UnitType type)
        {
            switch (faction)
            {
                case Faction.Player:
                    break;
                case Faction.Enemy:
                    CheckForDefeatTasks(type);
                    break;
            }
        }

        private void CheckForDefeatTasks(UnitType type)
        {
            CheckForGeneralDefeatTask();
            
            switch (type)
            {
                case UnitType.Light:
                    CheckForLightDefeatTask();
                    break;
                case UnitType.Medium:
                    CheckForMediumDefeatTask();
                    break;
                case UnitType.Heavy:
                    CheckForHeavyDefeatTask();
                    break;
            }
        }

        #region DefeatTasksCheck
        
        private void CheckForGeneralDefeatTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.DefeatEnemy))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void CheckForHeavyDefeatTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.DefeatHeavyEnemy))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void CheckForMediumDefeatTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.DefeatMediumEnemy))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void CheckForLightDefeatTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.DefeatLightEnemy))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }
        
        #endregion

        private void CheckForSpawnTasks(UnitType type)
        {
            CheckForGeneralSpawnTask();
            
            switch (type)
            {
                case UnitType.Light:
                    CheckForLightSpawnTask();
                    break;
                case UnitType.Medium:
                    CheckForMediumSpawnTask();
                    break;
                case UnitType.Heavy:
                    CheckForHeavySpawnTask();
                    break;
            }
        }

        #region CheckSpawnTasks

        private void CheckForLightSpawnTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.SpawnLightUnit))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void CheckForMediumSpawnTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.SpawnMediumUnit))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void CheckForHeavySpawnTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.SpawnHeavyUnit))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }


        private void CheckForGeneralSpawnTask()
        {
            if (NeedChangeProgressForType(DailyTaskType.SpawnUnit))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        void IFoodListener.OnFoodBalanceChanged(int value)
        {

        }

        void IFoodListener.OnFoodGenerated()
        {
            if (NeedChangeProgressForType(DailyTaskType.ProduceFood))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        #endregion

        private void OnTaskGenerated(DailyTask task)
        {
            _logger.Log($"CHECKER DAILY TASK IS UNLOCKED {CurrentTask.IsUnlocked}");
            DailyTaskUpdated?.Invoke(CurrentTask);
        }

        private void OnAdsWatchedChanged()
        {
            if (NeedChangeProgressForType(DailyTaskType.AdsWatch))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(1);
            }
        }

        private void OnCoinCollected(float amount)
        {
            if (NeedChangeProgressForType(DailyTaskType.EarnCoins))
            {
                _userContainer.DailyTaskStateHandler.AddProgress(amount);
            }
        }


        private void OnProgressChanged()
        {
            CurrentTask.Progress = DailyState.ProgressOnTask;
            CurrentTask.IsCompleted = (CurrentTask.Target - DailyState.ProgressOnTask)
                                       < Constants.ComparisonThreshold.MONEY_EPSILON;
            if (CurrentTask.Progress > CurrentTask.Target)
            {
                CurrentTask.Progress = CurrentTask.Target;
            }
            
            _logger.Log($"CHECKER DAILY TASK IS UNLOCKED {CurrentTask.IsUnlocked}");
            DailyTaskUpdated?.Invoke(CurrentTask);
        }

        private bool NeedChangeProgressForType(DailyTaskType type)
        {
            return 
                CurrentTask.Config.DailyTaskType == type && 
                CurrentTask.Progress < CurrentTask.Target &&
                !CurrentTask.IsRunOut &&
                CurrentTask.IsUnlocked;
            
        }
    }
}