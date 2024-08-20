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
using _Game.Core.Debugger;
using _Game.Core.Services.Analytics;
using _Game.Gameplay.BattleLauncher;
using Assets._Game.Gameplay._Timer.Scripts;
using Sirenix.OdinInspector;

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
            IGameInitializer gameInitializer)
        {
            _logger = logger;
            _timerService = timerService;
            _adsConfigRepository = adsConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
#if UNITY_ANDROID
            _rewardedID = AdsManager.Instance.MAXCustomSettings.RewardedID;
            _interstitialID = AdsManager.Instance.MAXCustomSettings.InterstitialID;
#endif
#if UNITY_IOS
            _rewardedID = _maxSettings.RewardedID_IOS;
            _interstitialID = _maxSettings.InterstitialID_IOS;
#endif
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
        }

        private bool IsInternetConnected() =>
            Application.internetReachability != NetworkReachability.NotReachable;

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
            throw new NotImplementedException();
        }

        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement)
        {
            throw new NotImplementedException();
        }
    }
}