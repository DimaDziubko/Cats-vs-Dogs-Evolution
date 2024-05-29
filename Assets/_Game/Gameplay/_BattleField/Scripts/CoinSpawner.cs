using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class CoinSpawner : ICoinSpawner
    {
        private readonly ICoinFactory _coinFactory;
        private readonly IAudioService _audioService;
        private readonly ICoinCounter _coinCounter;

        private Vector3 _coinCounterPosition;

        public CoinSpawner(
            ICoinFactory coinFactory,
            IAudioService audioService,
            ICoinCounter coinCounter)
        {
            _coinFactory = coinFactory;
            _audioService = audioService;
            _coinCounter = coinCounter;
            
        }

        public void Init(Vector3 coinCounterPosition) => 
            _coinCounterPosition = coinCounterPosition;

        void ICoinSpawner.SpawnLootCoin(Vector3 position, float amount)
        {
            _audioService.PlayCoinDropSound();
    
            var lootCoin = _coinFactory.GetLootCoin();
    
            if (lootCoin == null)
            {
                return;
            }
    
            lootCoin.Position = position;
            lootCoin.Init(amount, _coinCounterPosition);
            lootCoin.AnimationCompleted += OnCoinAnimationCompleted;
            lootCoin.Jump();
        }

        private void OnCoinAnimationCompleted(LootCoin lootCoin)
        {
            _audioService.PlayCoinCollectSound();
            
            _coinCounter.AddCoins(lootCoin.Amount);
            lootCoin.AnimationCompleted -= OnCoinAnimationCompleted;
        }
    }
}