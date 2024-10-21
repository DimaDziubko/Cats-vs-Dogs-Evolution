using System;
using _Game.Common;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Hud;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Debugger
{
    public class MyDebugger : MonoBehaviour
    {
        [Inject, ShowInInspector]
        private TimerService _timerService;
        
        [Inject, ShowInInspector]
        private UserContainer _userContainer;
        
        [Inject, ShowInInspector]
        private AdsGemsPackService _adsGemsPackService;

        [Inject, ShowInInspector]
        private Hud _hud;
        
        [Inject, ShowInInspector] private MaxAdsService _maxAdsService;

        public MaxTest MaxTest => MaxTest.I;
        
        [Inject, ShowInInspector]
        private MyLogger _logger;
        public void Start()
        {
            // _maxAdsService.OnVideoLoaded += OnVideoLoaded;
            // _maxAdsService.VideoLoadingFailed += OnVideoLoadingFailed;
            MaxTest.OnVideoLoaded += OnVideoLoaded;
            MaxTest.VideoLoadingFailed += OnVideoLoadingFailed;
            
            _hud.AdsDebugView.OnShowClicked += OnShowButtonClicked;
            _hud.AdsDebugView.OnLoadClicked += OnLoadButtonClicked;

            _hud.AdsDebugView.OnInterShowClicked += OnInterShowButtonClicked;
            _hud.AdsDebugView.OnInterLoadClicked += OnInterLoadButtonClicked;

            _hud.AdsDebugView.OnShowMediationDebuggerClicked += OnShowDebugger;
        }

        private void OnShowDebugger()
        {
            MaxTest.ShowDebugger();
        }

        private void OnVideoLoaded(AdType type)
        {
            switch (type)
            {
                case AdType.Rewarded:
                    _logger.Log($"Ad (rewarded) loaded {type.ToString()}", DebugStatus.Success);
                    _hud.AdsDebugView.SetStatus("<color=green>Ready</color>");
                    //_hud.AdsDebugView.SetReadyToShow(_maxAdsService.IsAdReady(AdType.Rewarded));
                    _hud.AdsDebugView.SetReadyToShow(MaxTest.IsAdReady(AdType.Rewarded));
                    break;
                case AdType.Interstitial:
                    _logger.Log($"Ad (inter) loaded {type.ToString()}", DebugStatus.Success);
                    _hud.AdsDebugView.SetInterStatus("<color=green>Ready</color>");
                    _hud.AdsDebugView.SetInterReadyToShow(MaxTest.IsAdReady(AdType.Interstitial));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public void OnDestroy()
        {
            //_maxAdsService.OnVideoLoaded -= OnVideoLoaded;
            MaxTest.OnVideoLoaded -= OnVideoLoaded;
            MaxTest.VideoLoadingFailed -= OnVideoLoadingFailed;
            
            _hud.AdsDebugView.OnShowClicked -= OnShowButtonClicked;
            _hud.AdsDebugView.OnLoadClicked -= OnLoadButtonClicked;
            
            //_maxAdsService.VideoLoadingFailed -= OnVideoLoadingFailed;

            _hud.AdsDebugView.OnInterShowClicked -= OnInterShowButtonClicked;
            _hud.AdsDebugView.OnInterLoadClicked -= OnInterLoadButtonClicked;
            
            _hud.AdsDebugView.OnShowMediationDebuggerClicked -= OnShowDebugger;
        }

        private void OnInterLoadButtonClicked()
        {
            //await _maxAdsService.LoadRewardedAd();
            MaxTest.LoadInterstitial();
            _hud.AdsDebugView.SetInterStatus("<color=yellow>Loading...</color>");
        }

        private void OnInterShowButtonClicked()
        {
            //_maxAdsService.ShowRewardedVideo(null, Placement.Food);
            MaxTest.ShowInterstitial();
            _hud.AdsDebugView.SetInterReadyToShow(MaxTest.IsAdReady(AdType.Interstitial));
            _hud.AdsDebugView.SetInterStatus("<color=green>Showing...</color>");
        }

        private void OnVideoLoadingFailed(AdType obj)
        {
            switch (obj)
            {
                case AdType.Rewarded:
                    _hud.AdsDebugView.SetStatus("<color=red>Failed...</color>");
                    break;
                case AdType.Interstitial:
                    _hud.AdsDebugView.SetInterStatus("<color=red>Failed...</color>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
            
        }

        private void OnShowButtonClicked()
        {
            //_maxAdsService.ShowRewardedVideo(null, Placement.Food);
            MaxTest.ShowRewardedAd();
            _hud.AdsDebugView.SetReadyToShow(MaxTest.IsAdReady(AdType.Rewarded));
            _hud.AdsDebugView.SetStatus("<color=green>Showing...</color>");
        }
        
        private async void OnLoadButtonClicked()
        {
            //await _maxAdsService.LoadRewardedAd();
            MaxTest.LoadRewardedAd();
            _hud.AdsDebugView.SetStatus("<color=yellow>Loading...</color>");
        }
    }
}