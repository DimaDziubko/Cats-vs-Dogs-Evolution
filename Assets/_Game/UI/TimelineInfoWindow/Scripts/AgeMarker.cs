using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI.TimelineInfoWindow.Scripts
{
    public class AgeMarker : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _imageHolder;
        [SerializeField] private Sprite _emptyMarker;
        [SerializeField] private Sprite _filledMarker;
        
        //Animation data
        [SerializeField] private float _animationScale = 1.2f;
        [SerializeField] private float _animationFade = 0.5f;
        [SerializeField] private int _loopCount = 1;
        
        public void Initialize(bool isFilled)
        {
            _imageHolder.sprite = isFilled ? _filledMarker : _emptyMarker;
        }

        public void PlayRippleAnimation(in float rippleAnimationDuration)
        {
            _imageHolder.sprite = _filledMarker;
            
            _rectTransform.DOScale(_animationScale, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
            _imageHolder.DOFade(_animationFade, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
        }
    }
}