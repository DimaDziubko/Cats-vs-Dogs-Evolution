using _Game.Core.Services.Audio;
using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._Coins.Scripts
{
    public class RewardCoin : Coin
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        //Animation data
        [SerializeField] private float _startAlphaValue = 0;
        [SerializeField] private float _startFadeDuration = 0;
        [SerializeField] private float _endAlphaValue = 1;
        [SerializeField] private float _endFadeDuration = 0.5f;

        [SerializeField] private float _randomDirectionMoveDuration = 0.5f;
        [SerializeField] private float _delayBetweenMovеments = 0.5f;
        [SerializeField] private float _moveToWalletDuration = 0.5f;

        private IAudioService _audioService;

        public void Init(IAudioService audioService)
        {
            _audioService = audioService;
            
            _spriteRenderer.DOFade(_startAlphaValue, _startFadeDuration).OnComplete(() =>
            {
                _spriteRenderer.DOFade(_endAlphaValue, _endFadeDuration); 
            });
        }

        public void StartMovement(Vector3 targetPoint, Vector3 startMovementVector)
        {
            Vector3 movePosition = new Vector3(startMovementVector.x, startMovementVector.y, 0) + Position;
            
            _transform.DOMove(movePosition, _randomDirectionMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                DOVirtual.DelayedCall(_delayBetweenMovеments, () =>
                {
                    _transform.DOMove(targetPoint, _moveToWalletDuration).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        _audioService.PlayFillingWalletSFX();
                        Recycle();
                    });
                });
            });
        }
    }
}