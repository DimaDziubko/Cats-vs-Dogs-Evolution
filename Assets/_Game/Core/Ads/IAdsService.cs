using System;
using Assets._Game.Core.Services.Analytics;

namespace Assets._Game.Core.Ads
{
    public interface IAdsService
    {
        event Action RewardedVideoLoaded;
        bool IsRewardedVideoReady { get; }
        event Action<AdImpressionDto> RewardedAdImpression;
        void ShowRewardedVideo(Action onVideoCompleted, RewardType placement);
    }
}