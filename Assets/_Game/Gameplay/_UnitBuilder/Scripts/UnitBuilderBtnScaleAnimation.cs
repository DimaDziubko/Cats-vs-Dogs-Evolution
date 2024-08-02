using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderBtnScaleAnimation : MonoBehaviour
    {
        private const float HALF = 0.5f;

        [SerializeField] private RectTransform _transform;
        [Space]
        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private float _animationDuration = 1f;
        [SerializeField] private float _targetScale = 1.2f;
        [SerializeField] private float _initialScale = 1;


        public void DoScaleAnimation()
        {
            if(!_isEnabled) return; 
            _transform.DOScale(_targetScale, _animationDuration * HALF)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _transform
                    .DOScale(_initialScale, _animationDuration * HALF)
                    .SetEase(Ease.InBack));
        }
    }
}
