using System;
using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Timer.Scripts;
using _Game.Utils.Timers;
using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Ads.ApplovinMaxAds
{
    public class MaxAdsService : IAdsService, IDisposable
    {
        private const string MaxSdkKey =
            "5AAhiuFzwRBZXL6NRkfMQIFE9TpJ-fX4qinXb1VVTh4_1ANSv1qJJ3TSWLnV_Jaq1LLcMr7rXCqTMC0FDqZXu6";
#if UNITY_IOS
        private readonly string _interstitialID = "fa1af5faa1b59bdc";
        private readonly string _rewardedID = "89c3dc86f476dee";

#elif UNITY_ANDROID
        private readonly string _interstitialID = "bf36589164e49496";
        private readonly string _rewardedID = "5500a0f67f9db05f";

        //private const string RewardedInterstitialAdUnitId = "ENTER_ANDROID_REWARD_INTER_AD_UNIT_ID_HERE";
        //private const string BannerAdUnitId = "39486b35f459019a";
        //private const string MRecAdUnitId = "ENTER_ANDROID_MREC_AD_UNIT_ID_HERE";

#endif

        public event Action<AdType> VideoLoaded;
        public event Action<AdType> VideoLoadingFailed;

        private Action _onVideoCompleted;

        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly ITimerService _timerService;
        private readonly IAdsConfigRepository _adsConfigRepository;
        private readonly IUserContainer _userContainer;

        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;


        public bool IsTimeForInterstitial { get; private set; }
        public float TimeLeft => _timer.TimeLeft;

        private GameTimer _timer;
        private int _rewardedRetryAttempt;
        private string _placement;
        private int _interstitialRetryAttempt;

        private SynchronizedCountdownTimer _countdownTimer;

        public bool CanShowInterstitial =>
            _adsConfigRepository.GetConfig().IsInterstitialActive &&
            (Purchases.BoughtIAPs?.Find(x => x.Count > 0) == null) &&
            IsInternetConnected() &&
            BattleStatistics.BattlesCompleted > _adsConfigRepository.GetConfig().InterstitialBattleTreshold;

        public MaxAdsService(
            IMyLogger logger,
            IUserContainer userContainer,
            ITimerService timerService,
            IConfigRepositoryFacade configRepositoryFacade,
            IGameInitializer gameInitializer
        )
        {
            _logger = logger;
            _timerService = timerService;
            _adsConfigRepository = configRepositoryFacade.AdsConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializeRewardedAds();
                InitializeInterstitialAds();
            };

            MaxSdk.InitializeSdk();

            LoadAndShowCmpFlow();

            Subscribe();
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;

            Unsubscribe();
        }

        public bool IsAdReady(AdType type)
        {
            switch (type)
            {
                case AdType.Rewarded:
                    return MaxSdk.IsRewardedAdReady(_rewardedID);

                case AdType.Interstitial:
                    return MaxSdk.IsInterstitialReady(_interstitialID);

                default:
                    return false;
            }
        }

        public void ShowInterstitialVideo(Placement placement)
        {
            _logger.Log("Inter_ ShowInterstitialVideo");
            if (IsTimeForInterstitial && CanShowInterstitial)
            {
                _logger.Log("Inter_ Can Show Ready");

                var delay = _adsConfigRepository.GetConfig().InterstitialDelay;
                StartCountdown(delay);

                MaxSdk.ShowInterstitial(_interstitialID);
            }
            else
            {
                _logger.Log("Inter_ Can't Show Inter");
            }
        }

        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement)
        {
            if (MaxSdk.IsRewardedAdReady(_rewardedID))
            {
                _logger.Log("Rewarded Video Show");
                _onVideoCompleted = onVideoCompleted;
                _placement = placement.ToString();
                var delay = _adsConfigRepository.GetConfig().RewardInterstitialDelay;
                StartCountdown(delay);

                MaxSdk.ShowRewardedAd(_rewardedID);
            }
            else
            {
                Debug.Log("Ad not ready");
            }
        }

        private void LoadAndShowCmpFlow()
        {
            var cmpService = MaxSdk.CmpService;

            cmpService.ShowCmpForExistingUser(error =>
            {
                if (null == error)
                {
                    // The CMP alert was shown successfully.
                }
            });
        }

        private void Subscribe()
        {
            if (MaxHelper.I.IsDebugAdMode)
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
                {
                    MaxSdk.ShowMediationDebugger();
                };
            }
        }

        private void Unsubscribe()
        {
        }

        private void StartCountdown(float delay)
        {
            _logger.Log($"START INTERSTITIAL COUNTDOWN! {delay}", DebugStatus.Warning);

            if (_timer != null)
            {
                _countdownTimer.Stop();
                IsTimeForInterstitial = false;
            }


            if (_countdownTimer == null)
            {
                _countdownTimer = new SynchronizedCountdownTimer(delay);
                _countdownTimer.TimerStop += OnInterstitialAdTimerOut;
            }

            _countdownTimer.Start();

            _logger.Log($"INTERSTITIAL READY: {IsTimeForInterstitial}!", DebugStatus.Warning);
        }

        private bool IsInternetConnected() =>
            Application.internetReachability != NetworkReachability.NotReachable;

        private void OnInterstitialAdTimerOut()
        {
            IsTimeForInterstitial = true;
        }

        private async UniTask RetryLoadWithDelay(Func<UniTask> loadFunc, double retryDelay)
        {
            _logger.Log($"Retrying in {retryDelay} seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(retryDelay));
            await loadFunc();
        }

        #region Rewarded Ad Methods

        private async void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            await LoadRewardedAd();
        }

        public async UniTask LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_rewardedID);
            _logger.Log("Rewarded ad loading...", DebugStatus.Success);
            await UniTask.CompletedTask;
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad loaded", DebugStatus.Success);
            VideoLoaded?.Invoke(AdType.Rewarded);
            _rewardedRetryAttempt = 0;
        }

        private async void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));

            _logger.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            VideoLoadingFailed?.Invoke(AdType.Rewarded);
            await RetryLoadWithDelay(LoadRewardedAd, retryDelay);
        }

        private async void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            await LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad displayed");
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad clicked");
        }

        private async void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad dismissed");
            await LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad received reward");
            _onVideoCompleted?.Invoke();
        }

        #endregion


        #region Interstitial Ad Methods

        private async void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

            await LoadInterstitial();
        }

        private async UniTask LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_interstitialID);
            _logger.Log("Interstitial AD Loading...");
            await UniTask.CompletedTask;
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial loaded", DebugStatus.Success);

            VideoLoaded?.Invoke(AdType.Interstitial);

            _interstitialRetryAttempt = 0;
        }

        private async void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            _logger.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            await RetryLoadWithDelay(LoadInterstitial, retryDelay);
        }

        private async void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            await LoadInterstitial();
        }

        private async void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial dismissed");
            await LoadInterstitial();
        }

        #endregion
    }
}