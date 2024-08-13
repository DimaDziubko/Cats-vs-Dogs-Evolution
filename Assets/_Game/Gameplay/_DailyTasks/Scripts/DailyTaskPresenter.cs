using System;
using _Game.Core.Data;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Services.UserContainer;
using Assets._Game.Gameplay._Units.Scripts;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public interface IDailyTaskPresenter
    {
        event Action<DailyTaskDto> DailyTaskUpdated;
        void CompleteTask();
        DailyTaskDto CurrentDto { get; }
    }

    public class DailyTaskPresenter : IDailyTaskPresenter, IInitializable, IDisposable
    {
        public event Action<DailyTaskDto> DailyTaskUpdated;
        
        private readonly IUserContainer _userContainer;
        private readonly IDailyTaskCompletionChecker _dailyTaskCompletionChecker;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeNavigator _ageNavigator;

        private readonly DailyTaskDto _currentDto = new DailyTaskDto();

        private string _dailyTaskAdditionalDescription;
        public DailyTaskDto CurrentDto => _currentDto;

        public DailyTaskPresenter(
            IUserContainer userContainer,
            IDailyTaskCompletionChecker dailyTaskCompletionChecker,
            IBattleNavigator battleNavigator,
            IGeneralDataPool generalDataPool,
            IAgeNavigator ageNavigator)
        {
            _userContainer = userContainer;
            _dailyTaskCompletionChecker = dailyTaskCompletionChecker;
            _battleNavigator = battleNavigator;
            _generalDataPool = generalDataPool;
            _ageNavigator = ageNavigator;
        }

        void IInitializable.Initialize()
        {
            _dailyTaskCompletionChecker.DailyTaskUpdated += OnDailyTaskUpdated;
            OnDailyTaskUpdated(_dailyTaskCompletionChecker.CurrentTask);
            _battleNavigator.BattleChanged += OnBattleChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
        }

        void IDisposable.Dispose()
        {
            _dailyTaskCompletionChecker.DailyTaskUpdated += OnDailyTaskUpdated;
            _battleNavigator.BattleChanged -= OnBattleChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
        }

        private void OnAgeChanged() => OnDailyTaskUpdated(_dailyTaskCompletionChecker.CurrentTask);

        private void OnBattleChanged() => OnDailyTaskUpdated(_dailyTaskCompletionChecker.CurrentTask);

        public void CompleteTask() => 
            _userContainer.DailyTaskStateHandler.CompleteDailyTask();

        private void OnDailyTaskUpdated(DailyTask task)
        {
            string additionalDescription = GetAdditionalDescription(task.Config.DailyTaskType);
            _currentDto.DailyInfo = $"Daily {task.CompletedCount}/{task.MaxCountPerDay}";
            _currentDto.Progress = $"{task.Config.Description} {additionalDescription} {task.Progress}/{task.Target}";
            _currentDto.Reward = task.Config.Reward.ToString();
            _currentDto.IsCompleted = task.IsCompleted;
            _currentDto.IsRunOut = task.IsRunOut;
            DailyTaskUpdated?.Invoke(_currentDto);
        }

        private string GetAdditionalDescription(DailyTaskType type)
        {
            switch (type)
            {
                case DailyTaskType.SpawnLightUnit:
                    return _generalDataPool
                        .AgeStaticData.ForUnit(UnitType.Light)
                        .Config.Name;
                case DailyTaskType.SpawnMediumUnit:
                    return _generalDataPool
                        .AgeStaticData.ForUnit(UnitType.Medium)
                        .Config.Name;
                case DailyTaskType.SpawnHeavyUnit:
                    return _generalDataPool
                        .AgeStaticData.ForUnit(UnitType.Heavy)
                        .Config.Name;
                case DailyTaskType.DefeatLightEnemy:
                    return _generalDataPool
                        .BattleStaticData.ForUnit(_battleNavigator.CurrentBattle, UnitType.Light)
                        .Config.Name;
                case DailyTaskType.DefeatMediumEnemy:
                    return _generalDataPool
                        .BattleStaticData.ForUnit(_battleNavigator.CurrentBattle, UnitType.Medium)
                        .Config.Name;
                case DailyTaskType.DefeatHeavyEnemy:
                    return _generalDataPool
                        .BattleStaticData.ForUnit(_battleNavigator.CurrentBattle, UnitType.Heavy)
                        .Config.Name;
                default:
                    return string.Empty;
            }
        }
    }
}