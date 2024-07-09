using System;
using _Game.Core.Configs.Models;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Ads;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay._Timer.Scripts;
using Assets._Game.UI._SpeedBoostBtn.Scripts;
using Assets._Game.Utils.Extensions;
using UnityEngine;

namespace Assets._Game.Core.Services._BattleSpeedService._Scripts
{
    public class BattleSpeedService : IBattleSpeedService, IDisposable
    {
        public event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        public event Action<GameTimer, bool> SpeedBoostTimerActivityChanged;

        private readonly IBattleSpeedManager _battleSpeedManager;
        private readonly IAdsService _adsService;
        private readonly IUserContainer _persistentData;
        private readonly IBattleSpeedConfigRepository _battleSpeedConfigRepository;
        private readonly IMyLogger _logger;
        private readonly ITimerService _timerService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;

        private IBattleSpeedStateReadonly BattleSpeed => _persistentData.State.BattleSpeedState;

        private int _maxAvailableSpeedId;
        private int _currentBattleSpeedId;


        private int CurrentBattleSpeedId
        {
            get => _currentBattleSpeedId;
            set
            {
                _currentBattleSpeedId = value > _maxAvailableSpeedId ? 0 : value;
                ChangeBattleSpeed(_currentBattleSpeedId);
            }
        }

        private bool _isBattleRunning;

        private readonly BattleSpeedBtnModel _battleSpeedBtnModel = new BattleSpeedBtnModel();

        public BattleSpeedService(
            IBattleSpeedManager battleSpeedManager,
            IAdsService adsService,
            IUserContainer persistentData,
            IBattleSpeedConfigRepository battleSpeedConfigRepository,
            IMyLogger logger,
            ITimerService timerService,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer)
        {
            _battleSpeedManager = battleSpeedManager;
            _adsService = adsService;
            _persistentData = persistentData;
            _battleSpeedConfigRepository = battleSpeedConfigRepository;
            _logger = logger;
            _timerService = timerService;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            var maxSpeedId = _battleSpeedConfigRepository.GetBattleSpeedConfigs().Count - 1;
            _maxAvailableSpeedId = Mathf.Min(BattleSpeed.PermanentSpeedId + 1, maxSpeedId); 
            
            PrepareBattleSpeed();
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged += OnSpeedChanged;
        }

        void IDisposable.Dispose()
        {
            _adsService.RewardedVideoLoaded -= OnRewardVideoLoaded;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged -= OnSpeedChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        public void OnBattleSpeedBtnClicked(BattleSpeedBtnState state)
        {
            switch (state)
            {
                case BattleSpeedBtnState.Active:
                    AttemptToShowRewardedVideo();
                    break;
                case BattleSpeedBtnState.Activated:
                    SwitchBattleSpeed();
                    break;
            }
        }

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

        private void AttemptToShowRewardedVideo()
        {
            if (_adsService.IsRewardedVideoReady)
            {
                _adsService.ShowRewardedVideo(OnSpeedBoostRewardedVideoComplete, RewardType.Speed);
            }
            else
            {
                _logger.LogWarning("Attempted to show video ad, but none was ready.");
            }
        }
        
        private void OnSpeedChanged(bool _) => UpdateBattleSpeedBtnModel();

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
            NotifyBattleSpeedBtnModelChanged();
        }

        private void UpdateSpeedInfo(BattleSpeedConfig speedConfig) => 
            _battleSpeedBtnModel.InfoText = $"x{speedConfig.SpeedFactor.ToInvariantString()}";

        private void UpdateSpeedButtonState()
        {
            if (!BattleSpeed.IsNormalSpeedActive)
            {
                _battleSpeedBtnModel.State = BattleSpeedBtnState.Activated;
            }
            else
            {
                _battleSpeedBtnModel.State = _adsService.IsRewardedVideoReady ? BattleSpeedBtnState.Active : BattleSpeedBtnState.Inactive;
                UpdateNextSpeedInfo();
            }
        }
        
        private void UpdateNextSpeedInfo()
        {
            if (CurrentBattleSpeedId < _maxAvailableSpeedId)
            {
                var boostedSpeedConfig = _battleSpeedConfigRepository.GetBattleSpeedConfig(CurrentBattleSpeedId + 1);
                _battleSpeedBtnModel.InfoText = $"x{boostedSpeedConfig.SpeedFactor.ToInvariantString()}";
            }
        }

        private void UpdateSpeedButtonTimer()
        {
            var timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer != null && !BattleSpeed.IsNormalSpeedActive)
            {
                _battleSpeedBtnModel.TimerTime = timer.TimeLeft;
            }
        }
        
        private void NotifyBattleSpeedBtnModelChanged()
        {
            _battleSpeedBtnModel.IsUnlocked = _featureUnlockSystem.IsFeatureUnlocked(Feature.BattleSpeed);
            BattleSpeedBtnModelChanged?.Invoke(_battleSpeedBtnModel);
        }

        private void OnSpeedBoostRewardedVideoComplete()
        {
            CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId + 1;
            CreateBattleTimer(_battleSpeedConfigRepository.GetBattleSpeedConfig(CurrentBattleSpeedId).Duration);
            TryNotifyAboutTimerActivity(true);
            if(_isBattleRunning) StartSpeedBoostTimer();
            _persistentData.ChangeNormalSpeed(false);
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

        private void SwitchBattleSpeed() => 
            CurrentBattleSpeedId++;

        private void OnRewardVideoLoaded() => 
            UpdateBattleSpeedBtnModel();

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

        private void ResetBattleSpeed()
        {
            TryNotifyAboutTimerActivity(false);
            CurrentBattleSpeedId = BattleSpeed.PermanentSpeedId;
            _persistentData.ChangeNormalSpeed(true);
            UpdateBattleSpeedBtnModel();
        }
        
        private void SaveBattleTimerValue()
        {
            GameTimer timer = _timerService.GetTimer(TimerType.BattleSpeed);
            if (timer != null)
            {
                _persistentData.ChangeBattleTimerDurationLeft(timer.TimeLeft);
            }
        }
    }
}