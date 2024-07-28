using System;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Analytics;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.Scenario;
using Assets._Game.Core._GameSaver;
using Assets._Game.Core._SystemUpdate;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Battle.Scripts
{
    public class Battle : IGameUpdate, IDisposable, IPauseHandler
    {
        public event Action BattleStarted;
        public event Action BattleStopped;
        public event Action<bool> BattlePaused;
        
        private readonly BattleField _battleField;

        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;
        private int _currentCachedWave;
        
        private AudioClip _bGM;
        
        private readonly IPauseManager _pauseManager;
        private readonly IAudioService _audioService;
        private readonly ISystemUpdate _systemUpdate;
        private readonly ICoinCounter _coinCounter;
        private readonly IBattleSpeedManager _speedManager;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IGameSaver _gameSaver;
        private readonly IAnalyticsService _analytics;
        private readonly IDTDAnalyticsService _dtdAnalytics;
        private readonly IBattlePresenter _battlePresenter;
        private IBattleMediator _battleMediator;

        private BattleAnalyticsData _battleAnalyticsData;
        public bool BattleInProcess { get; private set; }
        private bool IsPaused => _pauseManager.IsPaused;

        public Battle(
            IPauseManager pauseManager,
            IBattleSpeedManager speedManager,
            IAudioService audioService,
            ISystemUpdate systemUpdate,
            ICoinCounter coinCounter,
            BattleField battleField,
            IBattleSpeedService battleSpeed,
            IGameSaver gameSaver,
            IAnalyticsService analytics,
            IDTDAnalyticsService dtdAnalytics,
            IBattlePresenter battlePresenter)
        {
            _systemUpdate = systemUpdate;
            _speedManager = speedManager;
            _battleField = battleField;
            _coinCounter = coinCounter;
            _audioService = audioService;
            _pauseManager = pauseManager;
            _battleSpeed = battleSpeed;
            _gameSaver = gameSaver;
            _analytics = analytics;
            _dtdAnalytics = dtdAnalytics;
            _battlePresenter = battlePresenter;
        }

        public void Init()
        {
            _battleField.Init();
            
            _scenarioExecutor = new BattleScenarioExecutor();
            
            _battlePresenter.BattleDataUpdated += UpdateBattle;
            UpdateBattle(_battlePresenter.BattleData, false);

            _systemUpdate.Register(this);
            
            BattleStarted += OnBattleStarted;
            BattleStopped += OnBattleStopped;
            BattlePaused += _battleSpeed.OnBattlePaused;
        }

        public void Dispose()
        {
            _battlePresenter.BattleDataUpdated -= UpdateBattle;
            _systemUpdate.Unregister(this);
            
            BattleStarted -= OnBattleStarted;
            BattleStopped -= OnBattleStopped;
            BattlePaused -= _battleSpeed.OnBattlePaused;
            _pauseManager.UnRegister(this);
        }

        public void StartBattle()
        {
            AstarPath.active.Scan();
            
            BattleInProcess = true;
            _activeScenario = _scenarioExecutor.Begin(_battleField.UnitSpawner);
            _battleField.StartBattle();

            PlayAmbience();
            
            BattleStarted?.Invoke();
        }

        private void OnBattleStarted()
        {
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
            _battleSpeed.OnBattleStarted();
            _analytics.OnBattleStarted(_battleAnalyticsData);
            _dtdAnalytics.OnBattleStarted(_battleAnalyticsData);
        }

        public void ResetSelf()
        {
            UpdateBattle(_battlePresenter.BattleData, false);
            _battleField.ResetSelf();
        }
        
        void IGameUpdate.GameUpdate()
        {
            if(IsPaused) return;
            if (BattleInProcess)
            {
                var waves = _activeScenario.GetWaves();
                int currentWave = waves.currentWave;
                if (_currentCachedWave != currentWave && currentWave <= waves.wavesCount)
                {
                    _battleMediator.OnWaveChanged(currentWave, waves.wavesCount);
                    _dtdAnalytics.SendEvent($"Wave {currentWave}");
                    _analytics.SendEvent($"Wave {currentWave}");
                    _currentCachedWave = currentWave;
                }
                _activeScenario.Progress(_speedManager.CurrentSpeedFactor);
                _battleField.GameUpdate();
            }
        }

        public void Cleanup() => _battleField.Cleanup();

        public void StopBattle()
        {
            StopAmbience();
            BattleInProcess = false;
            BattleStopped?.Invoke();
        }
        
        private void OnBattleStopped()
        {
            _battleSpeed.OnBattleStopped();
            _gameSaver.OnBattleStopped();
        }
        
        private void UpdateBattle(BattleData data, bool needClearCache)
        {
            _bGM = data.Ambience;
            _battleField.UpdateBase(Faction.Enemy);
            _scenarioExecutor.UpdateScenario(data.ScenarioData.Scenario);
            _coinCounter.MaxCoinsPerBattle = data.ScenarioData.MaxCoinsPerBattle;
            _battleAnalyticsData = data.ScenarioData.AnalyticsData;
        }


        private void PlayAmbience()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Play(_bGM);
            }
        }

        private void StopAmbience()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Stop();
            }
        }

        void IPauseHandler.SetPaused(bool isPaused) => 
            BattlePaused?.Invoke(isPaused);

        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }
    }
}