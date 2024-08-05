using System;
using _Game.Core._GameListenerComposite;
using _Game.Core.CustomKernel;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Core.Services.Analytics;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay.Scenario;
using _Game.UI._BattleUIController;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using Zenject;

namespace _Game.Gameplay._Battle.Scripts
{
    public class Battle : 
        IGameTickable, 
        IDisposable, 
        IPauseListener, 
        IStartBattleListener, 
        IInitializable,
        IStopBattleListener,
        IBattleSpeedListener
    {
        private readonly BattleField _battleField;

        private readonly BattleAmbienceController _ambienceController;
        private readonly BattleAnalyticsController _analyticsController;
        
        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;
        private int _currentCachedWave;
        
        private readonly IAudioService _audioService;
        private readonly IBattlePresenter _battlePresenter;
        private readonly IMyLogger _logger;

        private readonly BattleUIController _uiController;

        private float _speedFactor;
        
        public Battle(
            IAudioService audioService,
            BattleField battleField,
            IAnalyticsService analytics,
            IDTDAnalyticsService dtdAnalytics,
            IBattlePresenter battlePresenter,
            BattleUIController uiController,
            IMyLogger logger)
        {
            _battleField = battleField;
            _audioService = audioService;
            _battlePresenter = battlePresenter;
            _ambienceController = new BattleAmbienceController(audioService);
            _analyticsController = new BattleAnalyticsController(analytics, dtdAnalytics);
            _uiController = uiController;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            _battleField.Init();
            
            _scenarioExecutor = new BattleScenarioExecutor();
            
            _battlePresenter.BattleDataUpdated += UpdateBattle;
            UpdateBattle(_battlePresenter.BattleData, false);
        }

        void IDisposable.Dispose()
        {
            _battlePresenter.BattleDataUpdated -= UpdateBattle;
        }

        void IStartBattleListener.OnStartBattle()
        {
            _analyticsController.SendStartData();
            
            AstarPath.active.Scan();
            
            _activeScenario = _scenarioExecutor.Begin(_battleField.UnitSpawner);
            _battleField.StartBattle();
            
            _ambienceController.PlayAmbience();
            _audioService.PlayStartBattleSound();
        }

        void IStopBattleListener.OnStopBattle() => 
            _ambienceController.StopAmbience();

        public void Reset()
        {
            UpdateBattle(_battlePresenter.BattleData, false);
            _battleField.ResetSelf();
        }

        public void Cleanup() => _battleField.Cleanup();

        void IGameTickable.Tick(float deltaTime)
        {
            var waves = _activeScenario.GetWaves();
            int currentWave = waves.currentWave;
            if (_currentCachedWave != currentWave && currentWave <= waves.wavesCount)
            {
                _uiController.UpdateWave(currentWave, waves.wavesCount);
                _analyticsController.SendWave($"Wave {currentWave}");
                _currentCachedWave = currentWave;
            }

            _activeScenario.Progress(deltaTime * _speedFactor);
            _battleField.GameUpdate(deltaTime);
        }


        private void UpdateBattle(BattleData data, bool needClearCache)
        {
            _battleField.UpdateBase(Faction.Enemy);
            _scenarioExecutor.UpdateScenario(data.ScenarioData.Scenario);
            _ambienceController.UpdateAmbience(data.Ambience);
            _analyticsController.UpdateData(data.ScenarioData.AnalyticsData);
        }
        

        void IPauseListener.SetPaused(bool isPaused) => 
            _battleField.SetPaused(isPaused);

        void IBattleSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            _speedFactor = speedFactor;
            _battleField.SetSpeedFactor(speedFactor);
        }
    }
}