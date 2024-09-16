using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class UIElementsFadeAnimation : MonoBehaviour
    {
        [SerializeField] private float _startAlphaValue;
        [SerializeField] private float _targetAlphaValue;
        [SerializeField] private float _fadeDelay;
        [SerializeField] private float _fadeDuration;

        [SerializeField] private Image[] _images;
        [SerializeField] private TMP_Text[] _labels;

        private Tween _imageTween;
        private Tween _labelTween;
        private Tween _fadeDelayTween;
        private Tween _fadeDurationTween;
        public void Play()
        {
            _imageTween?.Kill();
            _labelTween?.Kill();
            _fadeDelayTween?.Kill();
            _fadeDurationTween?.Kill();
            
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            
            if (_images != null)
            {
                foreach (var image in _images)
                {
                    if (image != null)
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, _startAlphaValue);
                    }
                }
            }

            if (_labels != null)
            {
                foreach (var label in _labels)
                {
                    if (label != null)
                    {
                        label.color = new Color(label.color.r, label.color.g, label.color.b, _startAlphaValue);
                    }
                }
            }

            _fadeDelayTween = DOVirtual.DelayedCall(_fadeDelay, () =>
            {
                if (_images != null)
                {
                    foreach (var image in _images)
                    {
                        if (image != null)
                        {
                            _imageTween = image.DOFade(_targetAlphaValue, _fadeDuration);
                        }
                    }
                }

                if (_labels != null)
                {
                    foreach (var label in _labels)
                    {
                        if (label != null)
                        {
                            _labelTween = label.DOFade(_targetAlphaValue, _fadeDuration);
                        }
                    }
                }

                _fadeDurationTween = DOVirtual.DelayedCall(_fadeDuration, () =>
                {
                    if (gameObject.activeInHierarchy)
                    {
                        gameObject.SetActive(false);
                    }
                });
            });
        }

        public void Cleanup()
        {
            _imageTween?.Kill();
            _labelTween?.Kill();
            _fadeDelayTween?.Kill();
            _fadeDurationTween?.Kill();

            _imageTween = null;
            _labelTween = null;
            _fadeDelayTween = null;
            _fadeDurationTween = null;
            
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
    }
}