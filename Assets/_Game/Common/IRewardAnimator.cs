using UnityEngine;

namespace _Game.Common
{
    public interface IRewardAnimator
    {
        void PlayCoins(Vector3 animationTargetPoint, float coinsCount, float coinsRatio);
    }
}