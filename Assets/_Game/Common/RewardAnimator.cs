﻿using System.Collections;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Common
{
    public class RewardAnimator : IRewardAnimator
    {
        private const float SPAWN_DELAY = 0.15f;
        private const int MIN_OBJECTS_AMOUNT = 15;
        private const int MAX_OBJECTS_AMOUNT = 25;
        
        private FloatRange _movementDistance = new FloatRange(0.5f, 1f);
        
        
        private readonly ICoinFactory _coinFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IRandomService _random;
        private readonly IAudioService _audioService;

        public RewardAnimator(
            ICoinFactory coinFactory, 
            ICoroutineRunner coroutineRunner,
            IRandomService random,
            IAudioService audioService)
        {
            _coinFactory = coinFactory;
            _coroutineRunner = coroutineRunner;
            _random = random;
            _audioService = audioService;
        }

        public void PlayCoins(Vector3 animationTargetPoint)
        {
            int coinsCount = _random.Next(MIN_OBJECTS_AMOUNT, MAX_OBJECTS_AMOUNT);
            _coroutineRunner.StartCoroutine(SpawnCoins(coinsCount, animationTargetPoint));
            
            _audioService.PlayCoinAppearanceSFX();
        }

        private IEnumerator SpawnCoins(int count, Vector3 targetPoint)
        {
            for (int i = 0; i < count; i++)
            {
                RewardCoin rewardCoin = _coinFactory.GetRewardCoin();
                rewardCoin.Init(_audioService);

                Vector3 randomDirection = _random.OnUnitSphere();
                Vector3 movementVector = randomDirection * _movementDistance.RandomValueInRange;
                rewardCoin.StartMovement(targetPoint, movementVector);
                yield return new WaitForSeconds(SPAWN_DELAY);
            }

        }
    }
}