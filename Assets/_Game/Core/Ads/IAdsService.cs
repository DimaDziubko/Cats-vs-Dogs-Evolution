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
#endif
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        event Action<AdType> VideoLoaded;
        event Action<AdType> VideoLoadingFailed;
        bool IsAdReady(AdType type);
        void ShowInterstitialVideo(Placement placement);
    }
}