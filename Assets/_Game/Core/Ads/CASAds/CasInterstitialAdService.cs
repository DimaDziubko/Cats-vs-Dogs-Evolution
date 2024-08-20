using System;
using _Game.Common;
using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
#if cas_advertisment_enabled
using CAS;
#endif
namespace _Game.Core.Ads.CASAds
{
#if cas_advertisment_enabled

    public class CasInterstitialAdService
    {
        public event Action<AdType> VideoLoaded;
        public event Action<AdImpressionDto> AdImpression;

        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private IMediationManager _manager;

        private bool _isVideoReady;
        private Placement _placement;
        public bool IsVideoReady => _isVideoReady;

        public CasInterstitialAdService(
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _logger = logger;
            _userContainer = userContainer;
        }
        
        public void Register(IMediationManager manager)
        {
            _manager = manager;
            manager.OnInterstitialAdLoaded += OnInterstitialAdLoaded;
            manager.OnInterstitialAdFailedToLoad += OnInterstitialAdFailedToLoad;
            manager.OnInterstitialAdShown += OnInterstitialAdShown;
            manager.OnInterstitialAdFailedToShow += OnInterstitialAdFailedToShow;
            manager.OnInterstitialAdImpression += OnInterstitialAdImpression;
            manager.OnInterstitialAdClicked += OnInterstitialAdClicked;
            manager.OnInterstitialAdClosed += OnInterstitialAdClosed;
            manager.LoadAd(AdType.Interstitial);
        }

        public void UnRegister(IMediationManager manager)
        {
            manager.OnInterstitialAdLoaded -= OnInterstitialAdLoaded;
            manager.OnInterstitialAdFailedToLoad -= OnInterstitialAdFailedToLoad;
            manager.OnInterstitialAdShown -= OnInterstitialAdShown;
            manager.OnInterstitialAdFailedToShow -= OnInterstitialAdFailedToShow;
            manager.OnInterstitialAdImpression -= OnInterstitialAdImpression;
            manager.OnInterstitialAdClicked -= OnInterstitialAdClicked;
            manager.OnInterstitialAdClosed -= OnInterstitialAdClosed;
        }

        private void OnInterstitialAdLoaded()
        {
            _logger.Log($"CAS Ad loaded {_manager.IsReadyAd(AdType.Interstitial)}");
            _isVideoReady = _manager.IsReadyAd(AdType.Interstitial);
            VideoLoaded?.Invoke(AdType.Interstitial);
        }

        private void OnInterstitialAdFailedToLoad(AdError error)
        {
            _logger.LogError($"CAS OnInterstitialAdFailedToLoad {error} ");
            _isVideoReady = _manager.IsReadyAd(AdType.Interstitial);
            VideoLoaded?.Invoke(AdType.Interstitial);
            _manager.LoadAd(AdType.Interstitial);
        }

        private void OnInterstitialAdShown()
        {
            _userContainer.AnalyticsStateHandler.AddAdsReviewed();
            _logger.Log($"CAS OnInterstitialAdShown");
        }

        private void OnInterstitialAdFailedToShow(string error) => 
            _logger.LogError($"CAS OnInterstitialAdFailedToShow {error} ");

        private void OnInterstitialAdImpression(AdMetaData meta)
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

        private void OnInterstitialAdClicked() => 
            _logger.Log($"CAS OnInterstitialAdClicked");


        private void OnInterstitialAdClosed() => 
            _logger.Log($"CAS OnInterstitialAdClosed");

        public void ShowVideo(Placement placement)
        {
            if (!IsVideoReady)
            {
                _logger.LogWarning("Attempted to show interstitial video before it was ready.");
                return;
            }
            
            _manager.ShowAd(AdType.Interstitial);
            _placement = placement;
        }
    }
#endif
}