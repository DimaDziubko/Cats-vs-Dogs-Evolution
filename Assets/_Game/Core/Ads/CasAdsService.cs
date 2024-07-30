using System;
using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Pause.Scripts;
using CAS;
using UnityEngine;

namespace _Game.Core.Ads
{
    public class CasAdsService : IAdsService, IDisposable
    {
        public event Action<AdImpressionDto> AdImpression;
        public event Action<AdType> VideoLoaded;
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private readonly CasRewardAdService _rewardAdsService;
        private readonly CasInterstitialAdService _interstitialAdsService;

        private IMediationManager _manager;

        public CasAdsService(
            IMyLogger logger,
            IPauseManager pauseManager,
            IUserContainer userContainer,
            IGameInitializer gameInitializer
            )
        {
            _rewardAdsService = new CasRewardAdService(logger, pauseManager, userContainer);
            _interstitialAdsService = new CasInterstitialAdService(logger, userContainer);
            _gameInitializer = gameInitializer;
            _logger = logger;

            _rewardAdsService.AdImpression += OnAdImpression;
            _rewardAdsService.VideoLoaded += OnVideoLoaded;
            _interstitialAdsService.AdImpression += OnAdImpression;
            _interstitialAdsService.VideoLoaded += OnVideoLoaded;
            
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _manager = MobileAds.BuildManager()
                .WithInitListener((success, error) =>
                {
                    _rewardAdsService.Register(_manager);
                    _interstitialAdsService.Register(_manager);
                    _logger.Log("success: " + success);

                }).Initialize();
            
        }

        public void Dispose()
        {
            _rewardAdsService.UnRegister(_manager);
            _interstitialAdsService.UnRegister(_manager);
            
            _rewardAdsService.AdImpression -= OnAdImpression;
            _rewardAdsService.VideoLoaded -= OnVideoLoaded;
            _interstitialAdsService.AdImpression -= OnAdImpression;
            _interstitialAdsService.VideoLoaded -= OnVideoLoaded;
            
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

        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement) => 
            _rewardAdsService.ShowRewardedVideo(onVideoCompleted, placement);

        public void ShowInterstitialVideo(Placement placement) => 
            _interstitialAdsService.ShowVideo(placement);

        private bool IsInternetConnected() => 
            Application.internetReachability != NetworkReachability.NotReachable;

        private void OnVideoLoaded(AdType type) => 
            VideoLoaded?.Invoke(type);

        private void OnAdImpression(AdImpressionDto dto) => 
            AdImpression?.Invoke(dto);
    }
}