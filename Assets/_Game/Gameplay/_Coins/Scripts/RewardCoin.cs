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

        [SerializeField] private float _firstMoveDuration = 0.5f;
        [SerializeField] private float _delayBetweenMovеments = 0.5f;
        [SerializeField] private float _lastMoveDuration = 1f;
        
        public void Init()
        {
            _spriteRenderer.DOFade(_startAlphaValue, _startFadeDuration).OnComplete(() =>
            {
                _spriteRenderer.DOFade(_endAlphaValue, _endFadeDuration); 
            });
        }
        
        public void StartMovement(Vector3 targetPoint, Vector3 startMovementVector)
        {
            Vector3 movePosition = new Vector3(startMovementVector.x, startMovementVector.y, 0) + Position;
            
            _transform.DOMove(movePosition, _firstMoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                DOVirtual.DelayedCall(_delayBetweenMovеments, () =>
                {
                    _transform.DOMove(targetPoint, _lastMoveDuration).SetEase(Ease.InQuad).OnComplete(Recycle);
                });
            });
        }
    }
}