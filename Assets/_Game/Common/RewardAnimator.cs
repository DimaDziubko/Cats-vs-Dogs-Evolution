using System.Collections;
using _Game.Core.Services.Audio;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Common
{
    public class RewardAnimator : IRewardAnimator
    {
        private const float SPAWN_DELAY = 0.1f;
        private const int MIN_OBJECTS_AMOUNT = 4;
        private const int MAX_OBJECTS_AMOUNT = 20;
        
        private FloatRange _movementDistance = new FloatRange(0.5f, 1f);
        
        
        private readonly ICoinFactory _coinFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IRandomService _random;
        private readonly IAudioService _audioService;
        private readonly IPersistentDataService _persistentData;

        public RewardAnimator(
            ICoinFactory coinFactory, 
            ICoroutineRunner coroutineRunner,
            IRandomService random,
            IAudioService audioService,
            IPersistentDataService persistentData)
        {
            _coinFactory = coinFactory;
            _coroutineRunner = coroutineRunner;
            _random = random;
            _audioService = audioService;
            _persistentData = persistentData;
        }

        public void PlayCoins(Vector3 animationTargetPoint, float totalCoins, float coinsRatio)
        {
            int coinsToSpawn = CalculateCoinsToSpawn(coinsRatio);
            float coinsPerCoin = totalCoins / coinsToSpawn;

            PlayCoinsSound();
            _coroutineRunner.StartCoroutine(SpawnCoins(coinsToSpawn, coinsPerCoin, animationTargetPoint));
        }

        private void PlayCoinsSound() => 
            _audioService.PlayCoinAppearanceSFX();

        private int CalculateCoinsToSpawn(float ratio) => 
            Mathf.CeilToInt(Mathf.Lerp(MIN_OBJECTS_AMOUNT, MAX_OBJECTS_AMOUNT, ratio));

        private IEnumerator SpawnCoins(int count, float coinsPerCoin, Vector3 targetPoint)
        {
            for (int i = 0; i < count; i++)
            {
                RewardCoin rewardCoin = _coinFactory.GetRewardCoin();
                rewardCoin.Init(_audioService, coinsPerCoin, _persistentData);

                Vector3 randomDirection = _random.OnUnitSphere();
                Vector3 movementVector = randomDirection * _movementDistance.RandomValueInRange;
                rewardCoin.StartMovement(targetPoint, movementVector);
                yield return new WaitForSeconds(SPAWN_DELAY);
            }
        }
    }
}