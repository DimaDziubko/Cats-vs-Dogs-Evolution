using System;
using _Game.Common;
using _Game.Core.Services.Analytics;
#if cas_advertisment_enabled
using CAS;
#endif

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
#if cas_advertisment_enabled
        event Action<AdType> VideoLoaded;
        event Action<AdImpressionDto> AdImpression;
        bool IsAdReady(AdType type);
#endif
        event Action<AdType> VideoLoaded;

        bool IsRewardAdReady();
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        void ShowInterstitialVideo(Placement placement);
    }
}