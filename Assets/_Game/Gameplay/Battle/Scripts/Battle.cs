using System;
using Assets._Game.Core._GameSaver;
using Assets._Game.Core._SystemUpdate;
using Assets._Game.Core.DataPresenters.BattlePresenter;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services._BattleSpeedService._Scripts;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Scenario;
using Assets._Game.UI._Environment;
using UnityEngine;

namespace Assets._Game.Gameplay.Battle.Scripts
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
            EnvironmentController environmentController,
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
            _environmentController = environmentController;
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
            UpdateBattle(_battlePresenter.BattleData);

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
            _battleSpeed.OnBattleStarted();
            _analytics.OnBattleStarted(_battleAnalyticsData);
            _dtdAnalytics.OnBattleStarted(_battleAnalyticsData);
        }

        public void ResetSelf()
        {
            UpdateBattle(_battlePresenter.BattleData);
            _battleField.ResetSelf();
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
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

        private void StopBGM()
        {
            if (_audioService != null && _bGM != null)
            {
                _audioService.Stop();
            }
        }

        void IPauseHandler.SetPaused(bool isPaused) => 
            BattlePaused?.Invoke(isPaused);

        public void SetMediator(IBattleMediator battleMediator){}
    }
}