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
        event Action<AdImpressionDto> AdImpression;
#endif
        event Action<AdType> VideoLoaded;
        bool IsAdReady(AdType type);

        bool IsRewardAdReady();
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        void ShowInterstitialVideo(Placement placement);
    }
}