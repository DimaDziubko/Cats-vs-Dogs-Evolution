using System;
using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Core.Pause.Scripts;
#if cas_advertisment_enabled
using CAS;
#endif
namespace _Game.Core.Ads.CASAds
{
#if cas_advertisment_enabled
    public class CasRewardAdService
    {
        public event Action<AdImpressionDto> AdImpression;
        public event Action<AdType> VideoLoaded;

        private bool _isRewardedVideoReady;
        public bool IsRewardedVideoReady => _isRewardedVideoReady;

        private readonly IGameInitializer _gameInitializer;
        private readonly IBattleManager _battleManager;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private Action _onVideoCompleted;
        private Placement _placement;

        private IMediationManager _manager;

        public CasRewardAdService(
            IMyLogger logger,
            IBattleManager battleManager,
            IUserContainer userContainer)
        {
            _logger = logger;
            _battleManager = battleManager;
            _userContainer = userContainer;
        }
        
        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement)
        {
            if (!IsRewardedVideoReady)
            {
                _logger.LogWarning("Attempted to show rewarded video before it was ready.");
                return;
            }
            
            _battleManager.SetPaused(true);
            _manager.ShowAd(AdType.Rewarded);
            _onVideoCompleted = onVideoCompleted;
            _placement = placement;
        }
        
        public void Register(IMediationManager manager)
        {
            _manager = manager;
            manager.OnRewardedAdLoaded += OnRewardedAdLoaded;
            manager.OnRewardedAdFailedToLoad += OnRewardedAdFailedToLoad;
            manager.OnRewardedAdCompleted += OnRewardedAdCompleted;
            manager.OnRewardedAdClosed += OnRewardedAdClosed;
            manager.OnRewardedAdImpression += OnRewardedAdImpression;
            manager.LoadAd(AdType.Rewarded);
        }

        public void UnRegister(IMediationManager manager)
        {
            manager.OnRewardedAdLoaded -= OnRewardedAdLoaded;
            manager.OnRewardedAdFailedToLoad -= OnRewardedAdFailedToLoad;
            manager.OnRewardedAdCompleted -= OnRewardedAdCompleted;
            manager.OnRewardedAdClosed -= OnRewardedAdClosed;
            manager.OnRewardedAdImpression -= OnRewardedAdImpression;
        }


        private void OnRewardedAdLoaded()
        {
            _logger.Log($"CAS Ad loaded {_manager.IsReadyAd(AdType.Rewarded)}");
            _isRewardedVideoReady = _manager.IsReadyAd(AdType.Rewarded);
            VideoLoaded?.Invoke(AdType.Rewarded);
        }

        private void OnRewardedAdFailedToLoad(AdError error)
        {
            _logger.LogError($"CAS OnRewardedAdFailedToLoad {error} ");
            _isRewardedVideoReady = _manager.IsReadyAd(AdType.Rewarded);
            VideoLoaded?.Invoke(AdType.Rewarded);
            _manager.LoadAd(AdType.Rewarded);
        }

        private void OnRewardedAdCompleted()
        {
            if (_battleManager.IsPaused) _battleManager.SetPaused(false);
            _onVideoCompleted?.Invoke();
            _onVideoCompleted = null;
            _manager.LoadAd(AdType.Rewarded);

            _userContainer.AnalyticsStateHandler.AddAdsReviewed();
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

            AdImpression?.Invoke(adImpressionDto);
        }

        private void OnRewardedAdClosed()
        {
            _logger.LogWarning($"CAS OnRewardedVideoClosed");
            _manager.LoadAd(AdType.Rewarded);
        }
        
    }
#endif
}