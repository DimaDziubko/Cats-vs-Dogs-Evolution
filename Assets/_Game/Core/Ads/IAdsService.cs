using System;
using _Game.Core.Services.Analytics;

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
        void Init();
        event Action RewardedVideoLoaded;
        bool IsRewardedVideoReady { get; }
        event Action<AdImpressionDto> RewardedAdImpression;
        void ShowRewardedVideo(Action onVideoCompleted, RewardType placement);
    }
}