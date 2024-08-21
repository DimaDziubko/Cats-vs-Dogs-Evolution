using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Gameplay._Timer.Scripts;
using Assets._Game.Core.UserState;
using MAXHelper;
using System;
using UnityEngine;
using _Game.Gameplay.BattleLauncher;
using UnityEngine.Events;
using Assets._Game.Gameplay._Timer.Scripts;
using Zenject;
using _Game.Core.Ads.ApplovinMaxAds;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Ads.ApplovinMaxAds
{
    public class MaxAdsService : IAdsService, IDisposable
    {
        public event Action<AdType> OnVideoLoaded;

        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly ITimerService _timerService;
        private readonly IAdsConfigRepository _adsConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IMadPixelAdsManager _madPixelAdsManager;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;


        public bool IsTimeForInterstitial { get; private set; }
        public float TimeLeft => _timer.TimeLeft;

        private GameTimer _timer;

        private string _rewardedID = "empty";
        private string _interstitialID = "empty";


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
            IGameInitializer gameInitializer,
            IMadPixelAdsManager madPixelAdsManager
            )
        {
            _logger = logger;
            _timerService = timerService;
            _adsConfigRepository = adsConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _madPixelAdsManager = madPixelAdsManager;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            //TODO
#if UNITY_ANDROID
            _rewardedID = AdsManager.Instance.MAXCustomSettings.RewardedID;
            _interstitialID = AdsManager.Instance.MAXCustomSettings.InterstitialID;
#endif
#if UNITY_IOS
            _rewardedID = _maxSettings.RewardedID_IOS;
            _interstitialID = _maxSettings.InterstitialID_IOS;
#endif

            _madPixelAdsManager.InitApplovin();

            WaitForAdsManagerInit().Forget();
        }

        void IDisposable.Dispose()
        {
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

                AdsManager.ShowInter(placement.ToString());
                var delay = _adsConfigRepository.GetConfig().InterstitialDelay;
                StartCountdown(delay);
            }
            else
            {
                _logger.Log("Inter_ Can't Show Inter");
            }
        }

        public void ShowRewardedVideo(UnityAction<bool> onVideoCompleted, Placement placement)
        {
            AdsManager.ShowRewarded(null, onVideoCompleted, placement.ToString());
            var delay = _adsConfigRepository.GetConfig().RewardInterstitialDelay;
            StartCountdown(delay);
        }

        private async UniTask WaitForAdsManagerInit()
        {
            await UniTask.WaitUntil(() => AdsManager.Ready());
            Subscribe();
            //TODO
            //LoadMainScene();
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
    }
}