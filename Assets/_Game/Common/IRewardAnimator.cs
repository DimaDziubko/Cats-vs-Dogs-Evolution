using UnityEngine;

namespace Assets._Game.Common
{
    public interface IRewardAnimator
    {
        void PlayCoins(Vector3 animationTargetPoint, float coinsRatio);
    }
}