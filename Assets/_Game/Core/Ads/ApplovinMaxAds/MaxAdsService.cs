using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Gameplay._Timer.Scripts;
using Assets._Game.Core.UserState;
using System;
using UnityEngine;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Gameplay._Timer.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Ads.ApplovinMaxAds
{
    public class MaxAdsService : IAdsService, IDisposable
    {
        private const string MaxSdkKey = "5AAhiuFzwRBZXL6NRkfMQIFE9TpJ-fX4qinXb1VVTh4_1ANSv1qJJ3TSWLnV_Jaq1LLcMr7rXCqTMC0FDqZXu6";
#if UNITY_IOS
        private readonly string _interstitialID = "fa1af5faa1b59bdc";
        private readonly string _rewardedID = "89c3dc86f476dee";

#else // UNITY_ANDROID
        private readonly string _interstitialID = "bf36589164e49496";
        private readonly string _rewardedID = "5500aOf67f9db05f";

        //private const string RewardedInterstitialAdUnitId = "ENTER_ANDROID_REWARD_INTER_AD_UNIT_ID_HERE";
        //private const string BannerAdUnitId = "39486b35f459019a";
        //private const string MRecAdUnitId = "ENTER_ANDROID_MREC_AD_UNIT_ID_HERE";

#endif

        public event Action<AdType> OnVideoLoaded;
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

        public bool CanShowInterstitial =>
            _adsConfigRepository.GetConfig().IsInterstitialActive &&
            (Purchases.BoughtIAPs?.Find(x => x.Count > 0) == null) &&
            IsInternetConnected() &&
            BattleStatistics.BattlesCompleted > _adsConfigRepository.GetConfig().InterstitialBattleTreshold;

        public MaxAdsService(
            IMyLogger logger,
            IBattleManager battleManager,
            IUserContainer userContainer,
            ITimerService timerService,
            IAdsConfigRepository adsConfigRepository,
            IGameInitializer gameInitializer
            )
        {
            _logger = logger;
            _timerService = timerService;
            _adsConfigRepository = adsConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;

            //if (_isDebugTest)
            //{
            //MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            //{
            //    // Show Mediation Debugger
            //    MaxSdk.ShowMediationDebugger();
            //};
            //}

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                _logger.Log("MAX SDK Initialized");

                InitializeInterstitialAds();
                InitializeRewardedAds();
            };
        }

        private void Init()
        {
            _logger.Log("MAX SDK START Initialize");

            MaxSdk.InitializeSdk();

            LoadAndShowCmpFlow();

            Subscribe();
        }

        void IDisposable.Dispose()
        {
            //if (_isDebugTest)
            //{
            //    MaxSdkCallbacks.OnSdkInitializedEvent -= (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            //    {
            //        // Show Mediation Debugger
            //        MaxSdk.ShowMediationDebugger();
            //    };
            //}

            MaxSdkCallbacks.OnSdkInitializedEvent -= sdkConfiguration =>
            {
                InitializeInterstitialAds();
                InitializeRewardedAds();
            };

            _gameInitializer.OnPostInitialization -= Init;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRWVideoLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterVideoLoaded;
        }

        public bool IsAdReady(AdType type)
        {
#if UNITY_EDITOR
            return true;
#endif
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
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRWVideoLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterVideoLoaded;
        }

        private void StartCountdown(float delay)
        {
            _logger.Log($"START INTERSTITIAL COUNTDOWN! {delay}");
            IsTimeForInterstitial = false;

            GameTimer timer = _timerService.GetTimer(TimerType.InterstitialAdDelay);
            if (timer != null)
            {
                timer.Stop();
                _timerService.RemoveTimer(TimerType.InterstitialAdDelay);
            }

            TimerData timerData = new TimerData
            {
                Countdown = true,
                Duration = delay,
                StartValue = delay
            };

            _timerService.CreateTimer(TimerType.InterstitialAdDelay, timerData, OnInterstitialAdTimerOut);
            _timerService.StartTimer(TimerType.InterstitialAdDelay);
            _timer = _timerService.GetTimer(TimerType.InterstitialAdDelay);

            _logger.Log($"INTERSTITIAL READY: {IsTimeForInterstitial}!");
        }

        private bool IsInternetConnected() =>
            Application.internetReachability != NetworkReachability.NotReachable;

        //TODO
        private void OnRWVideoLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoLoaded?.Invoke(AdType.Rewarded);

            _logger.Log("OnRewardedAdLoadedEvent invoked");
        }
        private void OnInterVideoLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoLoaded?.Invoke(AdType.Interstitial);

            _logger.Log("OnRewardedAdLoadedEvent invoked");
        }

        private void OnInterstitialAdTimerOut() =>
            IsTimeForInterstitial = true;


        private async UniTaskVoid StartAdCountdown(Action actionAfterDelay, float retryDelay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(retryDelay));
            actionAfterDelay?.Invoke();
        }

        #region Rewarded Ad Methods

        private void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            Debug.Log("RewardedAd Loading...");
            MaxSdk.LoadRewardedAd(_rewardedID);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            Debug.Log("Rewarded ad loaded");

            // Reset retry attempt
            _rewardedRetryAttempt = 0;
        }

        //TODO
        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));

            //rewardedStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            StartAdCountdown(LoadRewardedAd, (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("Rewarded ad dismissed");
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            Debug.Log("Rewarded ad received reward");
            _onVideoCompleted?.Invoke();

        }

        #endregion


        #region Interstitial Ad Methods

        private void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        void LoadInterstitial()
        {
            Debug.Log("Interstitia AD Loading...");
            MaxSdk.LoadInterstitial(_interstitialID);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("Interstitial loaded");

            // Reset retry attempt
            _interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
            Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            StartAdCountdown(LoadInterstitial, (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("Interstitial dismissed");
            LoadInterstitial();
        }

        #endregion

    }
}