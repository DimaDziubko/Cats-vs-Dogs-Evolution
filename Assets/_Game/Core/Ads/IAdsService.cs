using System;
using _Game.Common;

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
        event Action<string, MaxSdkBase.AdInfo> OnAdRevenuePaidEvent;
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        event Action<AdType> VideoLoaded;
        bool IsAdReady(AdType type);
        void ShowInterstitialVideo(Placement placement);
    }
}