using System;
using _Game.Core._GameInitializer;
using _Game.Core.Data;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay.Battle.Scripts;

namespace _Game.Core.DataPresenters.BattlePresenter
{
    public class BattlePresenter : IBattlePresenter, IDisposable
    {
        public event Action<BattleData> BattleDataUpdated;
        
        private readonly IBattleNavigator _navigator;
        private readonly IGeneralDataPool _dataPool;
        private readonly IGameInitializer _gameInitializer;

        public BattleData BattleData { get; } = new BattleData();

        public BattlePresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IGameInitializer gameInitializer)
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            UpdateBattleData();
            _navigator.BattleChanged += OnBattleChanged;
        }

        private void OnBattleChanged() => UpdateBattleData();

        private void UpdateBattleData()
        {
            BattleData.Battle = _navigator.CurrentBattle;
            BattleData.Ambience = _dataPool.BattleStaticData.ForAmbience(_navigator.CurrentBattle);
            BattleData.EnvironmentData = _dataPool.BattleStaticData.ForEnvironment(_navigator.CurrentBattle);
            BattleData.ScenarioData = _dataPool.BattleStaticData.ForBattleScenario(_navigator.CurrentBattle);
            BattleDataUpdated?.Invoke(BattleData);
        }

        void IDisposable.Dispose()
        {
            _navigator.BattleChanged -= OnBattleChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }
    }
}