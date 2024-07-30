using System;
using _Game.Common;
using _Game.Core.Services.Analytics;
using CAS;

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
        event Action<AdType> VideoLoaded;
        event Action<AdImpressionDto> AdImpression;
        bool IsAdReady(AdType type);
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        void ShowInterstitialVideo(Placement placement);
    }
}