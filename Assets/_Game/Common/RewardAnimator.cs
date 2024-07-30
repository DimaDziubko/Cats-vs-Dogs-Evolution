using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Coins.Factory;
using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Common
{
    public class RewardAnimator : IRewardAnimator
    {
        private const int MIN_OBJECTS_AMOUNT = 6;
        private const int MAX_OBJECTS_AMOUNT = 40;

        private readonly ICoinFactory _coinFactory;
        private readonly IAudioService _audioService;

        public RewardAnimator(
            ICoinFactory coinFactory,
            IAudioService audioService)
        {
            _coinFactory = coinFactory;
            _audioService = audioService;
        }

        public void PlayCoins(Vector3 animationTargetPoint, float coinsRatio)
        {
            int coinsToSpawn = CalculateCoinsToSpawn(coinsRatio);
            PlayCoinsSound();

            RewardCoinVFX coins = _coinFactory.GetRewardCoinVfx();
            coins.Init(Vector3.zero, animationTargetPoint, coinsToSpawn);
        }

        private void PlayCoinsSound() => 
            _audioService.PlayCoinAppearanceSFX();

        private int CalculateCoinsToSpawn(float ratio) => 
            Mathf.CeilToInt(Mathf.Lerp(MIN_OBJECTS_AMOUNT, MAX_OBJECTS_AMOUNT, ratio));
    }
}