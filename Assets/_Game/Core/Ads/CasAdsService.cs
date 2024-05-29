using System;
using _Game.Core._Logger;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Analytics;
using CAS;
using UnityEngine;

namespace _Game.Core.Ads
{
    public class CasAdsService : IAdsService, IDisposable
    {
        public event Action<AdImpressionDto> RewardedAdImpression;
        public event Action RewardedVideoLoaded;

        private bool _isRewardedVideoReady;
        public bool IsRewardedVideoReady => _isRewardedVideoReady && IsInternetConnected();

        private IMediationManager _manager;

        private readonly IPauseManager _pauseManager;

        private readonly IMyLogger _logger;

        private Action _onVideoCompleted;
        private RewardType _placement;

        public CasAdsService(
            IMyLogger logger,
            IPauseManager pauseManager)
        {
            _logger = logger;
            _pauseManager = pauseManager;
        }

        public void Init()
        {
            _manager = MobileAds.BuildManager()
                .WithInitListener((success, error) =>
                {
                    _manager.OnRewardedAdLoaded += OnRewardedAdLoaded;
                    _manager.OnRewardedAdFailedToLoad += OnRewardedAdFailedToLoad;
                    _manager.OnRewardedAdCompleted += OnRewardedAdCompleted;
                    _manager.OnRewardedAdClosed += OnRewardedAdClosed;
                    _manager.OnRewardedAdImpression += OnRewardedAdImpression;
                    _manager.LoadAd(AdType.Rewarded);
                    Debug.Log("success: " + success);
                }).Initialize();
        }

        public void Dispose()
        {
            _manager.OnRewardedAdLoaded -= OnRewardedAdLoaded;
            _manager.OnRewardedAdFailedToLoad -= OnRewardedAdFailedToLoad;
            _manager.OnRewardedAdCompleted -= OnRewardedAdCompleted;
            _manager.OnRewardedAdClosed -= OnRewardedAdClosed;
            _manager.OnRewardedAdImpression -= OnRewardedAdImpression;
        }

        public void ShowRewardedVideo(Action onVideoCompleted, RewardType placement)
        {
            if (!IsRewardedVideoReady)
            {
                _logger.LogWarning("Attempted to show rewarded video before it was ready.");
                return;
            }

            _pauseManager.SetPaused(true);
            _manager.ShowAd(AdType.Rewarded);
            _onVideoCompleted = onVideoCompleted;
            _placement = placement;
        }

        private bool IsInternetConnected() => 
            Application.internetReachability != NetworkReachability.NotReachable;

        private void OnRewardedAdLoaded()
        {
            _logger.Log($"CAS Ad loaded {_manager.IsReadyAd(AdType.Rewarded)}");
            _isRewardedVideoReady = _manager.IsReadyAd(AdType.Rewarded);
            RewardedVideoLoaded?.Invoke();
        }

        private void OnRewardedAdFailedToLoad(AdError error)
        {
            _logger.LogError($"CAS OnRewardedAdFailedToLoad {error} ");
            _isRewardedVideoReady = _manager.IsReadyAd(AdType.Rewarded);
            RewardedVideoLoaded?.Invoke();
            _manager.LoadAd(AdType.Rewarded);
        }

        private void OnRewardedAdCompleted()
        {
            if(_pauseManager.IsPaused) _pauseManager.SetPaused(false);
            _onVideoCompleted?.Invoke();
            _onVideoCompleted = null;
            _manager.LoadAd(AdType.Rewarded);
        }


        private void OnRewardedAdImpression(AdMetaData meta)
        {
            AdImpressionDto adImpressionDto = new AdImpressionDto()
            {
                Network = meta.network.ToString(),
                Placement = _placement,
                Revenue = meta.revenue,
                UnitId = meta.identifier
            };
            
            RewardedAdImpression?.Invoke(adImpressionDto);
        }

        private void OnRewardedAdClosed()
        {
            _logger.LogWarning($"CAS OnRewardedVideoClosed");
            _manager.LoadAd(AdType.Rewarded);
        }
    }
}