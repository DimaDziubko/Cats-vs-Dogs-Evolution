using Assets._Game.Core.Services.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardViewAppearanceAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _cardBgTransform;
        [SerializeField] private float _newCardScale;
        [SerializeField] private float _scaleAnimationDuration;

        [SerializeField] private Image _rippleImage;
        [SerializeField] private RectTransform _rippleTransform;
        [SerializeField] private float _rippleAnimationDuration;
        [SerializeField] private float _rippleScale = 1.3f;

        [SerializeField] private ImageFlashEffect _appearanceImageFlash;
        [SerializeField] private ImageAlphaFlashEffect _rippleImageAlphaFlash;

        [SerializeField] private AudioClip _cardAppearanceSfx;
        [SerializeField] private AudioClip _cardIlluminationSfx;
        
        private IAudioService _audioService;
        
        public void Init(IAudioService audioService, Color flashColor)
        {
            _audioService = audioService;
            _appearanceImageFlash.Init();
            _rippleImageAlphaFlash.Init(flashColor);
        }

        public void Play(bool needIlluminationAnimation)
        {
            if (!needIlluminationAnimation)
            {
                PlaySimple();
                return;
            }

            PlayWithRipple();
        }

        private void PlaySimple()
        {
            _appearanceImageFlash.TriggerFlash();
            _audioService.PlayOneShot(_cardAppearanceSfx);
        }

        private void PlayWithRipple()
        {
            _appearanceImageFlash.TriggerFlash(OnAppearanceFinished);
        }

        private void OnAppearanceFinished()
        {
            PlayRippleAnimation();
            _audioService.PlayOneShot(_cardIlluminationSfx);
        }

        private void PlayRippleAnimation()
        {
            _cardBgTransform.DOScale(_newCardScale, _scaleAnimationDuration / 2)
                .OnComplete(() =>
                    _cardBgTransform.DOScale(1, _scaleAnimationDuration / 2));
            
            _rippleImage.enabled = true;
            _rippleImageAlphaFlash.TriggerFlash();
            _rippleTransform.DOScale(_rippleScale, _rippleAnimationDuration);
            
            DOVirtual.DelayedCall(_rippleAnimationDuration, () => _rippleImage.enabled = false);
        }
    }
}