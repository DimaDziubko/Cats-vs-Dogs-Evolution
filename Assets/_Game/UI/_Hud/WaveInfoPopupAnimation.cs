using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud
{
    public class WaveInfoPopupAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Image _fadableImage;
        [SerializeField] private TextMeshProUGUI _fadableText;

        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _mainPosition;

        [SerializeField] private float _moveToMainTimeSec = 2;
        [SerializeField] private float _mainPositionDelay = 3;
        [SerializeField] private float _fadeTime = 2;

        private float _normalAlpha = 1f;
        private float _zeroAlpha = 0f;

        public void PlayAnimation()
        {
            gameObject.SetActive(true); 
            
            var imageColor = _fadableImage.color;
            imageColor.a = _zeroAlpha;
            _fadableImage.color = imageColor;

            var textColor = _fadableText.color;
            textColor.a = _zeroAlpha;
            _fadableText.color = textColor;

            _transform.localPosition = _startPosition;
            
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(_transform.DOLocalMove(_mainPosition, _moveToMainTimeSec).SetEase(Ease.OutQuad));
            
            sequence.Join(_fadableImage.DOFade(_normalAlpha, _moveToMainTimeSec));
            sequence.Join(_fadableText.DOFade(_normalAlpha, _moveToMainTimeSec));
            
            sequence.AppendInterval(_mainPositionDelay);

            sequence.Join(_fadableImage.DOFade(_zeroAlpha, _fadeTime));
            sequence.Join(_fadableText.DOFade(_zeroAlpha, _fadeTime));
            
            sequence.OnComplete(() => gameObject.SetActive(false));

            sequence.Play();
        }
    }
}
