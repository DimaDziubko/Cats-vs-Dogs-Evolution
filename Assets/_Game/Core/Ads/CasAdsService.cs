using System;
using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Debugger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Gameplay._Timer.Scripts;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Core._Logger;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Timer.Scripts;
using CAS;
using UnityEngine;
using Zenject;

namespace _Game.Core.Ads
{
    public class CasAdsService : IAdsService,IDisposable
    {
        public event Action<AdImpressionDto> AdImpression;
        public event Action<AdType> VideoLoaded;
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly ITimerService _timerService;
        private readonly IAdsConfigRepository _adsConfigRepository;
        private readonly IUserContainer _userContainer;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;

        private readonly CasRewardAdService _rewardAdsService;
        private readonly CasInterstitialAdService _interstitialAdsService;

        private IMediationManager _manager;

        private bool _isTimeForInterstitial;
        public bool IsTimeForInterstitial => _isTimeForInterstitial; 

        public bool CanShowInterstitial => 
            _adsConfigRepository.GetConfig().IsInterstitialActive &&
            (Purchases.BoughtIAPs?.Find(x => x.Count > 0) == null) &&
            IsInternetConnected();

        public CasAdsService(
            IMyLogger logger,
            IBattleManager battleManager,
            IUserContainer userContainer,
            ITimerService timerService,
            IAdsConfigRepository adsConfigRepository,
            IMyDebugger debugger,
            IGameInitializer gameInitializer)
        {
            _rewardAdsService = new CasRewardAdService(logger, battleManager, userContainer);
            _interstitialAdsService = new CasInterstitialAdService(logger, userContainer);
            _logger = logger;
            _timerService = timerService;
            _adsConfigRepository = adsConfigRepository;
            _userContainer = userContainer;
            debugger.CasAdsService = this;

            _rewardAdsService.AdImpression += OnAdImpression;
            _rewardAdsService.VideoLoaded += OnVideoLoaded;
            _interstitialAdsService.AdImpression += OnAdImpression;
            _interstitialAdsService.VideoLoaded += OnVideoLoaded;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _manager = MobileAds.BuildManager()
                .WithInitListener((success, error) =>
                {
                    _rewardAdsService.Register(_manager);
                    _interstitialAdsService.Register(_manager);
                    _isTimeForInterstitial = true;
                    _logger.Log("success: " + success);

                }).Initialize();

            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleCountChanged;

        }

        private void OnCompletedBattleCountChanged()
        {
            if (BattleStatistics.BattlesCompleted % 
                _adsConfigRepository.GetConfig().InterstitialBattleTreshold == 0 
                && IsAdReady(AdType.Interstitial))
            {
                ShowInterstitialVideo(Placement.BattleTreshold);
            }
        }

        void IDisposable.Dispose()
        {
            _rewardAdsService.UnRegister(_manager);
            _interstitialAdsService.UnRegister(_manager);
            
            _rewardAdsService.AdImpression -= OnAdImpression;
            _rewardAdsService.VideoLoaded -= OnVideoLoaded;
            _interstitialAdsService.AdImpression -= OnAdImpression;
            _interstitialAdsService.VideoLoaded -= OnVideoLoaded;
            
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleCountChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }


        public bool IsAdReady(AdType type)
        {
            if (!IsInternetConnected()) return false;
            
            switch (type)
            {
                case AdType.Banner:
                    break;
                case AdType.Interstitial:
                    return _interstitialAdsService.IsVideoReady;
                case AdType.Rewarded:
                    return _rewardAdsService.IsRewardedVideoReady;
                case AdType.AppOpen:
                    break;
                case AdType.Native:
                    break;
                case AdType.None:
                    break;
            }
            
            return false;
        }

        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement)
        {
            _rewardAdsService.ShowRewardedVideo(onVideoCompleted, placement);
            _isTimeForInterstitial = false;
            StartCountdown();
        }

        public void ShowInterstitialVideo(Placement placement)
        {
            if (_isTimeForInterstitial && CanShowInterstitial)
            {
                _interstitialAdsService.ShowVideo(placement);
                _isTimeForInterstitial = false;
                StartCountdown();
            }
        }

        private void StartCountdown()
        {
            GameTimer timer = _timerService.GetTimer(TimerType.InterstitialAdDelay);
            if (timer != null)
            {
                timer.Stop();
                _timerService.RemoveTimer(TimerType.InterstitialAdDelay);
            }

            var config = _adsConfigRepository.GetConfig();

            TimerData timerData = new TimerData
            {
                Countdown = true, 
                Duration = config.InterstitialDelay, 
                StartValue = config.InterstitialDelay
            };
            
            _timerService.CreateTimer(TimerType.InterstitialAdDelay, timerData, OnInterstitialAdTimerOut);
            _timerService.StartTimer(TimerType.InterstitialAdDelay);
        }

        private bool IsInternetConnected() => 
            Application.internetReachability != NetworkReachability.NotReachable;

        private void OnVideoLoaded(AdType type) => 
            VideoLoaded?.Invoke(type);

        private void OnAdImpression(AdImpressionDto dto) => 
            AdImpression?.Invoke(dto);

        private void OnInterstitialAdTimerOut() => 
            _isTimeForInterstitial = true;
    }
}