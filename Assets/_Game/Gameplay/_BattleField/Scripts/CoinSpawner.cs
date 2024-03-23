using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using _Game.Gameplay.CoinCounter.Scripts;
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


        private void CalculateLootCoinTargetPosition(RectTransform _coinCounterViewTransform)
        {
            Vector2 screenPoint = 
                RectTransformUtility.WorldToScreenPoint(
                    _cameraService.UICameraOverlay, 
                    _coinCounterViewTransform.position);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _coinCounterViewTransform,
                screenPoint, 
                _cameraService.UICameraOverlay, 
                out _coinTargetPosition);
        }
    }
}