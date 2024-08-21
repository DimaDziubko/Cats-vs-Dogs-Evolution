using System;
using _Game.Common;
using _Game.Core.Services.Analytics;
using UnityEngine.Events;
#if cas_advertisment_enabled
using CAS;
#endif

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
#if cas_advertisment_enabled
        event Action<AdImpressionDto> AdImpression;
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
#endif
        event Action<AdType> OnVideoLoaded;
        bool IsAdReady(AdType type);
        void ShowRewardedVideo(UnityAction<bool> onVideoCompleted, Placement placement);
        void ShowInterstitialVideo(Placement placement);
    }
}