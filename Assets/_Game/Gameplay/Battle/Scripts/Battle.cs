using System;
using _Game.Core._GameSaver;
using _Game.Core._SystemUpdate;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Scenario;
using _Game.UI._Environment;
using UnityEngine;

namespace _Game.Gameplay.Battle.Scripts
{
    public class Battle : IGameUpdate, IDisposable, IPauseHandler
    {
        public event Action BattleStarted;
        public event Action BattleStopped;
        public event Action<bool> BattlePaused;
        
        
        private readonly BattleField _battleField;
        private readonly EnvironmentController _environmentController;

        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;

        private AudioClip _bGM;

        private readonly IBattleStateService _battleState;
        private readonly IPauseManager _pauseManager;
        private readonly IAudioService _audioService;
        private readonly ISystemUpdate _systemUpdate;
        private readonly ICoinCounter _coinCounter;
        private readonly IBattleSpeedManager _speedManager;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IGameSaver _gameSaver;
        private readonly IAnalyticsService _analytics;
        private readonly IDTDAnalyticsService _dtdAnalytics;

        private BattleAnalyticsData _battleAnalyticsData;
        private int _currentBattle;
        public bool BattleInProcess { get; private set; }
        private bool IsPaused => _pauseManager.IsPaused;

        public Battle(
            IBattleStateService battleState,
            IPauseManager pauseManager,
            IBattleSpeedManager speedManager,
            IAudioService audioService,
            ISystemUpdate systemUpdate,
            ICoinCounter coinCounter,
            BattleField battleField,
            EnvironmentController environmentController,
            IBattleSpeedService battleSpeed,
            IGameSaver gameSaver,
            IAnalyticsService analytics,
            IDTDAnalyticsService dtdAnalytics)
        {
            _systemUpdate = systemUpdate;
            _speedManager = speedManager;
            _battleField = battleField;
            _coinCounter = coinCounter;

            _audioService = audioService;
            
            _battleState = battleState;
            _pauseManager = pauseManager;
            _environmentController = environmentController;
            _battleSpeed = battleSpeed;
            _gameSaver = gameSaver;
            _analytics = analytics;
            _dtdAnalytics = dtdAnalytics;
        }

        public void Init()
        {
            _battleField.Init();
            
            _scenarioExecutor = new BattleScenarioExecutor();

            _battleState.BattlePrepared += UpdateBattle;

            UpdateBattle(_battleState.GetCurrentBattleData());
            _systemUpdate.Register(this);
            
            BattleStarted += OnBattleStarted;
            BattleStopped += OnBattleStopped;
            BattlePaused += _battleSpeed.OnBattlePaused;
        }

        public void Dispose()
        {
            _battleState.BattlePrepared -= UpdateBattle;
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

            PlayBGM();
            
            BattleStarted?.Invoke();
        }

        private void OnBattleStarted()
        {
            _battleSpeed.OnBattleStarted();
            _analytics.OnBattleStarted(_battleAnalyticsData);
            _dtdAnalytics.OnBattleStarted(_battleAnalyticsData);
        }

        public void ResetSelf()
        {
            UpdateBattle(_battleState.GetCurrentBattleData());
            _battleField.ResetSelf();
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
        }

        public void PrepareNextBattle()
        {
            _battleState.OpenNextBattle(_currentBattle + 1);
        }

        void IGameUpdate.GameUpdate()
        {
            if(IsPaused) return;
            if (BattleInProcess)
            {
                _activeScenario.Progress(_speedManager.CurrentSpeedFactor);
                _battleField.GameUpdate();
            }
        }

        public void Cleanup()
        {
            _battleField.Cleanup();
        }

        public void StopBattle()
        {
            StopBGM();
            BattleInProcess = false;
            _pauseManager.SetPaused(true);
            BattleStopped?.Invoke();
        }
        
        private void OnBattleStopped()
        {
            _battleSpeed.OnBattleStopped();
            _gameSaver.OnBattleStopped();
        }
        
        private void UpdateBattle(BattleData data)
        {
            _environmentController.ShowEnvironment(data.EnvironmentData);
            _currentBattle = data.Battle;
            _bGM = data.BGM;
            _battleField.UpdateBase(Faction.Enemy);
            _scenarioExecutor.UpdateScenario(data.Scenario);
            _coinCounter.MaxCoinsPerBattle = data.MaxCoinsPerBattle;
            _battleAnalyticsData = data.AnalyticsData;
        }


        private void PlayBGM()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Play(_bGM);
            }
        }

        private void StopBGM()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Stop();
            }
        }

        public void SetMediator(IBattleMediator battleMediator)
        {
            
        }
        
        void IPauseHandler.SetPaused(bool isPaused) => 
            BattlePaused?.Invoke(isPaused);
    }
}