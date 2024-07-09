using System;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay._Battle.Scripts;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core.Data;

namespace _Game.Core.DataPresenters.BattlePresenter
{
    public class BattlePresenter : IBattlePresenter, IDisposable
    {
        public event Action<BattleData, bool> BattleDataUpdated;
        
        private readonly IBattleNavigator _navigator;
        private readonly IGeneralDataPool _dataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineNavigator _timelineNavigator;

        public BattleData BattleData { get; } = new BattleData();

        public BattlePresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IGameInitializer gameInitializer,
            ITimelineNavigator timelineNavigator)
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _gameInitializer = gameInitializer;
            _timelineNavigator = timelineNavigator;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            UpdateBattleData(false);
            _navigator.BattleChanged += OnBattleChanged;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
        }

        private void OnTimelineChanged()
        {
            UpdateBattleData(true);
        }

        private void OnBattleChanged()
        {
            UpdateBattleData(false);
        }

        private void UpdateBattleData(bool needClearCache)
        {
            BattleData.Battle = _navigator.CurrentBattle;
            BattleData.Ambience = _dataPool.BattleStaticData.ForAmbience(_navigator.CurrentBattle);
            BattleData.EnvironmentData = _dataPool.BattleStaticData.ForEnvironment(_navigator.CurrentBattle);
            BattleData.ScenarioData = _dataPool.BattleStaticData.ForBattleScenario(_navigator.CurrentBattle);
            BattleDataUpdated?.Invoke(BattleData, needClearCache);
        }

        void IDisposable.Dispose()
        {
            _navigator.BattleChanged -= OnBattleChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }
    }
}