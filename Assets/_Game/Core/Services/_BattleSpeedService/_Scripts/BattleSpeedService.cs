using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Debugger;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Hud._BattleSpeedView;
using _Game.Utils.Extensions;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Timer.Scripts;
using UnityEngine;

namespace _Game.Core.Services._BattleSpeedService._Scripts
{
    public class BattleSpeedService : IBattleSpeedService, IDisposable
    {
        public event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        public event Action<GameTimer, bool> SpeedBoostTimerActivityChanged;

        private readonly IBattleSpeedManager _battleSpeedManager;
        private readonly IUserContainer _userContainer;
        private readonly IBattleSpeedConfigRepository _battleSpeedConfigRepository;
        private readonly ITimerService _timerService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private IBattleSpeedStateReadonly BattleSpeed => _userContainer.State.BattleSpeedState;

        private int MaxAvailableSpeedId => 
            Mathf.Min(BattleSpeed.PermanentSpeedId + 1, 
                _battleSpeedConfigRepository.GetBattleSpeedConfigs().Count - 1);
        
        private int _currentBattleSpeedId;
        
        private int CurrentBattleSpeedId
        {
            get => _currentBattleSpeedId;
            set
            {
                if(BattleSpeed.IsNormalSpeedActive)
                    _currentBattleSpeedId = value > BattleSpeed.PermanentSpeedId ? 0 : value;
                else
                    _currentBattleSpeedId = value > MaxAvailableSpeedId ? 0 : value;

                ChangeBattleSpeed(_currentBattleSpeedId);
            }
        }

        private bool _isBattleRunning;

        private readonly BattleSpeedBtnModel _battleSpeedBtnModel = new BattleSpeedBtnModel();
        public BattleSpeedBtnModel BattleSpeedBtnModel => _battleSpeedBtnModel; 
        
        public BattleSpeedService(
            IBattleSpeedManager battleSpeedManager,
            IUserContainer userContainer,
            IBattleSpeedConfigRepository battleSpeedConfigRepository,
            IMyLogger logger,
            ITimerService timerService,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer,
            IMyDebugger debugger)
        {
            _battleSpeedManager = battleSpeedManager;
            _userContainer = userContainer;
            _battleSpeedConfigRepository = battleSpeedConfigRepository;
            _logger = logger;
            _timerService = timerService;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;
            debugger.BattleSpeedService = this;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            PrepareBattleSpeed();
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged += OnNormalSpeedChanged;
            BattleSpeed.PermanentSpeedChanged += OnPermanentSpeedChanged;
        }

        void IDisposable.Dispose()
        {
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged -= OnNormalSpeedChanged;
            BattleSpeed.PermanentSpeedChanged -= OnPermanentSpeedChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        public void OnBattleSpeedBtnClicked() => SwitchBattleSpeed();

        public void OnBattleStopped()
        {
            _isBattleRunning = false;
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            timer?.Stop();
            SaveBattleTimerValue();
        }

        public void OnBattlePaused(bool isPaused)
        {
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer == null) return;
            if (isPaused)
            {
                timer.Stop();
                return;
            }
            
            if(_isBattleRunning) timer.Start();
        }

        public void OnBattleSpeedBtnShown()
        {
            TryNotifyAboutTimerActivity(true);
            UpdateBattleSpeedBtnModel();
        }

        public void OnBattleStarted()
        {
            _isBattleRunning = true;
            StartSpeedBoostTimer();
        }

        private void SwitchBattleSpeed() => 
            CurrentBattleSpeedId++;

        private void OnPermanentSpeedChanged(int id)
        {
            CurrentBattleSpeedId = id;
            TryResetBoostedSpeed();
            UpdateBattleSpeedBtnModel();
        }

        private void TryResetBoostedSpeed()
        {
            if (!BattleSpeed.IsNormalSpeedActive)
            {
                _userContainer.ChangeNormalSpeed(true);
                if (_timerService.GetTimer(TimerType.BattleSpeed) != null)
                {
                    _timerService.RemoveTimer(TimerType.BattleSpeed);
                }
            }
        }

        private void OnNormalSpeedChanged(bool isNormal)
        {
            if (!isNormal)
            {
                ActivateSpeedBoost();
            }
            
            UpdateBattleSpeedBtnModel();
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.BattleSpeed)
            {
                UpdateBattleSpeedBtnModel();
            }
        }

        private void PrepareBattleSpeed()
        {
            if (!BattleSpeed.IsNormalSpeedActive)
            {
                CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId + 1;
                CreateBattleTimer(BattleSpeed.DurationLeft);
                return;
            }

            CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId;
        }

        private void ChangeBattleSpeed(int newSpeedId)
        {
            var speedConfig = _battleSpeedConfigRepository.GetBattleSpeedConfig(newSpeedId);
            _battleSpeedManager.SetSpeedFactor(speedConfig.SpeedFactor);
            UpdateBattleSpeedBtnModel();
        }


        private void UpdateBattleSpeedBtnModel()
        {
            var speedConfig = _battleSpeedConfigRepository.GetBattleSpeedConfig(CurrentBattleSpeedId);
            UpdateSpeedInfo(speedConfig);
            UpdateSpeedButtonState();
            UpdateSpeedButtonTimer();
            BattleSpeedBtnModelChanged?.Invoke(_battleSpeedBtnModel);
        }

        private void UpdateSpeedInfo(BattleSpeedConfig speedConfig) => 
            _battleSpeedBtnModel.InfoText = $"x{speedConfig.SpeedFactor.ToInvariantString()}";

        private void UpdateSpeedButtonState()
        {
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.BattleSpeed))
            {
                _battleSpeedBtnModel.State = BtnState.Locked;
                return;
            }
            
            if (!BattleSpeed.IsNormalSpeedActive)
            {
                _battleSpeedBtnModel.State = BtnState.Activated;
                return;
            }

            if (BattleSpeed.PermanentSpeedId == 0)
            {
                _battleSpeedBtnModel.State = BtnState.Inactive;
                return;
            }

            _battleSpeedBtnModel.State = BtnState.Active;
        }

        private void UpdateSpeedButtonTimer()
        {
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer != null && !BattleSpeed.IsNormalSpeedActive)
            {
                _battleSpeedBtnModel.TimerTime = timer.TimeLeft;
            }
        }


        private void ActivateSpeedBoost()
        {
            CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId + 1;
            CreateBattleTimer(_battleSpeedConfigRepository.GetBattleSpeedConfig(CurrentBattleSpeedId).Duration);
            TryNotifyAboutTimerActivity(true);
            if(_isBattleRunning) StartSpeedBoostTimer();
        }

        private void CreateBattleTimer(float duration)
        {
            var timerData = new TimerData()
            {
                Countdown = true,
                Duration = duration,
                StartValue = duration,
            };
            
            _timerService.CreateTimer(TimerType.BattleSpeed, timerData, ResetBattleSpeed);
        }

        private void ResetBattleSpeed()
        {
            TryNotifyAboutTimerActivity(false);
            CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId;
            _userContainer.ChangeNormalSpeed(true);
            UpdateBattleSpeedBtnModel();
        }

        private void StartSpeedBoostTimer()
        {
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            timer?.Start();
        }

        private void TryNotifyAboutTimerActivity(bool isActive)
        {
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer != null)
            {
                SpeedBoostTimerActivityChanged?.Invoke(timer, isActive);
            }
        }

        private void SaveBattleTimerValue()
        {
            GameTimer timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer != null)
            {
                _userContainer.ChangeBattleTimerDurationLeft(timer.TimeLeft);
            }
        }
    }
}