using System;

namespace _Game.Core.Ads
{
    public interface IAdsService 
    {
        event Action RewardedVideoLoaded;
        bool IsRewardedVideoReady { get; }
        void Init();
        void ShowRewardedVideo(Action onVideoCompleted);
    }
}