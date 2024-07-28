﻿using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Debugger;
using _Game.Core.Services.UserContainer;
using _Game.UI._Hud._BattleSpeedView;
using _Game.UI._Hud._SpeedBoostView.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Ads;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.UserState;
using UnityEngine;

namespace _Game.Core.Services._SpeedBoostService.Scripts
{
    public class SpeedBoostService : ISpeedBoostService, IDisposable
    {
        public event Action<SpeedBoostBtnModel> SpeedBoostBtnModelChanged;
        
        private readonly IAdsService _adsService;
        private readonly IUserContainer _userContainer;
        private readonly IBattleSpeedConfigRepository _battleSpeedConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;
        private IBattleSpeedStateReadonly BattleSpeed => _userContainer.State.BattleSpeedState;

        private int _maxAvailableSpeedId;
        
        private readonly SpeedBoostBtnModel _speedBoostBtnModel = new SpeedBoostBtnModel();
        public SpeedBoostBtnModel SpeedBoostBtnModel => _speedBoostBtnModel;

        public SpeedBoostService(
            IAdsService adsService,
            IUserContainer userContainer,
            IBattleSpeedConfigRepository battleSpeedConfigRepository,
            IMyLogger logger,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer,
            IMyDebugger debugger)
        {
            _adsService = adsService;
            _userContainer = userContainer;
            _battleSpeedConfigRepository = battleSpeedConfigRepository;
            _logger = logger;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;
            debugger.SpeedBoostService = this;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged += OnNormalSpeedChanged;
            BattleSpeed.PermanentSpeedChanged += OnPermanentSpeedChanged;
        }

        void IDisposable.Dispose()
        {
            _adsService.RewardedVideoLoaded -= OnRewardVideoLoaded;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            BattleSpeed.IsNormalSpeedActiveChanged -= OnNormalSpeedChanged;
            BattleSpeed.PermanentSpeedChanged -= OnPermanentSpeedChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnNormalSpeedChanged(bool _) => 
            UpdateSpeedBoostBtnModel();

        private void OnPermanentSpeedChanged(int _) => 
            UpdateSpeedBoostBtnModel();

        void ISpeedBoostService.OnSpeedBoostBtnClicked() => 
            AttemptToShowRewardedVideo();

        void ISpeedBoostService.OnSpeedBoostBtnShown() => 
            UpdateSpeedBoostBtnModel();
        
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
        
        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.BattleSpeed)
            {
                UpdateSpeedBoostBtnModel();
            }
        }
        
        private void UpdateSpeedBoostBtnModel()
        {
            UpdateSpeedInfo();
            UpdateSpeedButtonState();
            SpeedBoostBtnModelChanged?.Invoke(_speedBoostBtnModel);
        }

        private void UpdateSpeedInfo()
        {
            var nextSpeedId = BattleSpeed.PermanentSpeedId + 1;
            var maxSpeedId = _battleSpeedConfigRepository.GetBattleSpeedConfigs().Count - 1;
            var nextAvailableSpeedId = Mathf.Min(nextSpeedId, maxSpeedId); 
            
            var boostedSpeedConfig = _battleSpeedConfigRepository.GetBattleSpeedConfig(nextAvailableSpeedId);
            _speedBoostBtnModel.InfoText = $"x{boostedSpeedConfig.SpeedFactor.ToInvariantString()}";
        }

        private void UpdateSpeedButtonState()
        {
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.BattleSpeed) || SpeedConfigOut())
            {
                _speedBoostBtnModel.State = BtnState.Locked;
                return;
            }
            
            if (!BattleSpeed.IsNormalSpeedActive)
            {
                _speedBoostBtnModel.State = BtnState.Activated;
                return;
            }
            
            _speedBoostBtnModel.State = 
                _adsService.IsRewardedVideoReady 
                    ? BtnState.Active 
                    : BtnState.Inactive;
        }

        private bool SpeedConfigOut() => 
            BattleSpeed.PermanentSpeedId == _battleSpeedConfigRepository.GetBattleSpeedConfigs().Count - 1;

        private void OnSpeedBoostRewardedVideoComplete() => 
            _userContainer.ChangeNormalSpeed(false);

        private void OnRewardVideoLoaded() => 
            UpdateSpeedBoostBtnModel();
        
    }
}