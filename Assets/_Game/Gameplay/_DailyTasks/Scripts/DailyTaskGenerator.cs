using System;
using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.Utils;
using Assets._Game.Core.UserState;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public interface IDailyTaskGenerator
    {
        event Action<DailyTask> DailyTaskGenerated;
        DailyTask CurrentTask { get;}
    }

    public class DailyTaskGenerator : IDailyTaskGenerator, IInitializable, IDisposable
    {
        public event Action<DailyTask> DailyTaskGenerated;

        private readonly IDailyTaskConfigRepository _dailyTaskConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IRandomService _random;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IMyLogger _logger;

        private IDailyTasksStateReadonly DailyState => _userContainer.State.DailyTasksState;
        private IAdsWeeklyWatchStateReadonly AdsWeeklyWatchState => _userContainer.State.AdsWeeklyWatchState;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private readonly DailyTask _currentDailyTask = new DailyTask();
        public DailyTask CurrentTask => _currentDailyTask;

        public DailyTaskGenerator(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IRandomService random,
            IFeatureUnlockSystem featureUnlockSystem,
            IMyLogger logger)
        {
            _dailyTaskConfigRepository = configRepositoryFacade.DailyTaskConfigRepository;
            _userContainer = userContainer;
            _random = random;
            _featureUnlockSystem = featureUnlockSystem;
            _logger = logger;
        }
        
        void IInitializable.Initialize()
        {
            if (TimeToGenerateNewDailyTasks())
            {
                _userContainer.DailyTaskStateHandler.ClearCompleted();
                _userContainer.DailyTaskStateHandler.ChangeLastTimeGenerated(DateTime.UtcNow);
                GenerateNewDailyTask();
            }
            else if(NotGeneratedYet())
            {
                GenerateNewDailyTask();
            }
            else
            {
                RestoreDailyTask();
            }
            
            DailyState.TaskCompletedChanged += OnTaskCompleted;
            
            if(!_featureUnlockSystem.IsFeatureUnlocked(Feature.DailyTask))
                _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
        }

        private void RestoreDailyTask()
        {
            var tasks = _dailyTaskConfigRepository.GetDailyTaskConfigs();
            GenerateDailyTask(tasks[DailyState.CurrentTaskIdx]);
        }

        private void GenerateNewDailyTask()
        {
            var tasks = _dailyTaskConfigRepository.GetDailyTaskConfigs();
            int dailyTaskId = SelectRandomDailyTask(tasks);
            DailyTaskConfig selectedConfig = tasks[dailyTaskId];
            GenerateDailyTask(selectedConfig);
        }

        private bool TimeToGenerateNewDailyTasks()
        {
            DateTime lastTimeGenerated = DailyState.LastTimeGenerated;
            
            if (DateTime.UtcNow - lastTimeGenerated > 
                TimeSpan.FromMinutes(_dailyTaskConfigRepository.RecoverTimeMinutes))
            {
                return true;
            }

            return false;
        }

        private bool NotGeneratedYet() => DailyState.CurrentTaskIdx == -1;

        void IDisposable.Dispose()
        {
            DailyState.TaskCompletedChanged -= OnTaskCompleted;     
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.DailyTask)
            {
                GenerateNewDailyTask();
            }
        }

        private void OnTaskCompleted()
        {
            AddReward();
            GenerateNewDailyTask();

        }

        private void AddReward() => 
            _userContainer.CurrenciesHandler.AddGems(_currentDailyTask.Config.Reward, CurrenciesSource.DailyTask);

        private int SelectRandomDailyTask(List<DailyTaskConfig> configs)
        {
            List<int> ids = new List<int>();
            foreach (var config in configs)
            {
                if(DailyState.CompletedTasks.Contains(config.Id - 1)) continue; //Completed tasks are idx
                for (int i = 0; i < config.DropChance; i++)
                {
                    ids.Add(config.Id);
                }
            }

            return _random.Next(0, ids.Count);
        }

        private void GenerateDailyTask(DailyTaskConfig config)
        {
            int targetArgument = GetTargetArgumentForType(config.DailyTaskType);

            _userContainer.DailyTaskStateHandler.ChangeTaskIdx(config.Id);

            float target = SelectTarget(config, targetArgument);
            
            bool isCompleted = (target - DailyState.ProgressOnTask) < Constants.ComparisonThreshold.MONEY_EPSILON;

            _currentDailyTask.Config = config;
            _currentDailyTask.Progress = DailyState.ProgressOnTask;
            _currentDailyTask.Target = target;
            _currentDailyTask.IsCompleted = isCompleted;
            _currentDailyTask.MaxCountPerDay = _dailyTaskConfigRepository.MaxDailyCountPerDay;
            _currentDailyTask.CompletedCount = DailyState.CompletedTasks.Count;
            _currentDailyTask.IsRunOut =
                DailyState.CompletedTasks.Count >= _dailyTaskConfigRepository.MaxDailyCountPerDay;
            _currentDailyTask.IsUnlocked = _featureUnlockSystem.IsFeatureUnlocked(Feature.DailyTask);
            
            DailyTaskGenerated?.Invoke(_currentDailyTask);
        }

        private float SelectTarget(DailyTaskConfig config, int targetArgument)
        {
            return config.LinearFunctions.Count - 1 >= TimelineState.MaxBattle
                ? config.LinearFunctions[TimelineState.MaxBattle].GetValue(targetArgument) 
                : config.LinearFunctions[0].GetValue(targetArgument);
        }

        private int GetTargetArgumentForType(DailyTaskType dailyTaskType)
        {
            switch (dailyTaskType)
            {
                case DailyTaskType.AdsWatch:
                    return AdsWeeklyWatchState.LastWeekWatchedAds;
                default:
                    return TimelineState.FoodProductionLevel;
            }
        }
    }
}