using System;
using _Game.Common;

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        event Action<AdType> VideoLoaded;
        event Action<AdType> VideoLoadingFailed;
        bool IsAdReady(AdType type);
        void ShowInterstitialVideo(Placement placement);
    }
}