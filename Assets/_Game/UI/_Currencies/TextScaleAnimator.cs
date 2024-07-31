using _Game.Utils.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.UI._Currencies
{
    public class TextScaleAnimator : MonoBehaviour
    {
        [SerializeField] private float _scaleAnimationDuration = 0.1f;
        [SerializeField] private float _targetScale = 1.1f;
        [SerializeField] private float _normalScale = 1.0f;
        
        [SerializeField] private float _textAnimationDuration = 1f;
        [SerializeField] private float _textAnimationDelay = 1.5f;


        public void AnimateCurrenciesTextDelayed(TMP_Text label, double currentValue, double newValue)
        {
            DOVirtual.DelayedCall(_textAnimationDelay, () => AnimateCoinText(label, currentValue ,newValue));
        }

        private void AnimateCoinText(TMP_Text label, double currentValue, double newValue)
        {
            DOTween.To(() => currentValue, 
                    x => label.text = x.FormatMoney(),
                    newValue, 
                    _textAnimationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => currentValue = newValue);
        }

        public void PlayScaleAnimation(TMP_Text label)
        {
            label.transform.DOKill(); 
            label.transform.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(label.transform.DOScale(_targetScale, _scaleAnimationDuration/2))
                .Append(label.transform.DOScale(_normalScale, _scaleAnimationDuration/2));
            sequence.Play();
        }
    }
}
