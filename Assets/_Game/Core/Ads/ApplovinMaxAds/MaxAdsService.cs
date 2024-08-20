using _Game.Common;
using MAXHelper;
using System;
using UnityEngine;

namespace _Game.Core.Ads.ApplovinMaxAds
{
    public class MaxAdsService : IAdsService, IDisposable
    {

        public event Action<AdType> OnVideoLoaded;


        [SerializeField] private MAXCustomSettings _maxSettings;
        private string _rewardedID = "empty";
        private string _interstitialID = "empty";

        private void Init()
        {
#if UNITY_ANDROID
            _rewardedID = _maxSettings.RewardedID;
            _interstitialID = _maxSettings.InterstitialID;
#endif
#if UNITY_IOS
            _rewardedID = _maxSettings.RewardedID_IOS;
            _interstitialID = _maxSettings.InterstitialID_IOS;
#endif
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsAdReady(AdType type)
        {
            return MaxSdk.IsRewardedAdReady(""); //adUnitId

        }

        public bool IsRewardAdReady()
        {
            throw new NotImplementedException();
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