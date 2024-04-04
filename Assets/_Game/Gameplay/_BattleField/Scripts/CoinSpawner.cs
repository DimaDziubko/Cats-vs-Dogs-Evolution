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
        private readonly IWorldCameraService _cameraService;
        private readonly ICoinCounter _coinCounter;

        private Vector3 _coinTargetPosition;

        public CoinSpawner(
            ICoinFactory coinFactory,
            IAudioService audioService,
            IWorldCameraService cameraService,
            ICoinCounter coinCounter)
        {
            _coinFactory = coinFactory;
            _audioService = audioService;
            _cameraService = cameraService;
            _coinCounter = coinCounter;
            
        }

        public void Init(RectTransform coinCounterViewTransform)
        {
            CalculateLootCoinTargetPosition(coinCounterViewTransform);
            
            //TODO Delete
            Debug.Log($"Coin target position {_coinTargetPosition}");
        }
        
        void ICoinSpawner.SpawnLootCoin(Vector3 position, float amount)
        {
            _audioService.PlayCoinDropSound();
            
            var lootCoin =  _coinFactory.GetLootCoin();
            lootCoin.Position = position;
            
            lootCoin.Init(amount, _coinTargetPosition);
            
            lootCoin.AnimationCompleted += OnCoinAnimationCompleted;
            lootCoin.Jump();
        }

        private void OnCoinAnimationCompleted(LootCoin lootCoin)
        {
            _audioService.PlayCoinCollectSound();
            
            _coinCounter.AddCoins(lootCoin.Amount);
            lootCoin.AnimationCompleted -= OnCoinAnimationCompleted;
        }


        private void CalculateLootCoinTargetPosition(RectTransform coinCounterViewTransform)
        {
            Vector2 screenPoint = 
                RectTransformUtility.WorldToScreenPoint(
                    _cameraService.UICameraOverlay, 
                    coinCounterViewTransform.position);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                coinCounterViewTransform,
                screenPoint, 
                _cameraService.UICameraOverlay, 
                out _coinTargetPosition);
        }
    }
}