using _Game.Core.Services.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardViewAppearanceAnimation : MonoBehaviour
    {
        [SerializeField] private bool _useFlashAnimation = false; 
        
        [SerializeField] private RectTransform _cardBgTransform;
        [SerializeField] private float _newCardScale;
        [SerializeField] private float _scaleAnimationDuration;

        [SerializeField] private Image _rippleImage;
        [SerializeField] private RectTransform _rippleTransform;
        [SerializeField] private float _rippleAnimationDuration;
        [SerializeField] private Vector2 _rippleScale = new Vector2(1.5f, 1.4f);
        
        [SerializeField] private ImageFlashEffect _appearanceImageFlash;
        [SerializeField] private ImageMaskAnimation _maskAnimation;
        
        [SerializeField] private ImageAlphaFlashEffect _rippleImageAlphaFlash;

        [SerializeField] private ScaleAnimation _newNotifierScaleAnimation;
        
        [SerializeField] private AudioClip _cardAppearanceSfx;
        [SerializeField] private AudioClip _cardIlluminationSfx;
        
        private IAudioService _audioService;

        private bool _isNew;
        
        public void Init(IAudioService audioService, Color flashColor, bool isNew)
        {
            _audioService = audioService;
            _appearanceImageFlash.Init();
            _rippleImageAlphaFlash.Init(flashColor);
            _maskAnimation.Init();
            _newNotifierScaleAnimation.Init();
            _isNew = isNew;
        }

        public void Play(
            bool needIlluminationAnimation)
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
            if(_useFlashAnimation)
                _appearanceImageFlash.TriggerFlash();
            else
            {
                _maskAnimation.TriggerMask();
            }
            
            _audioService.PlayOneShot(_cardAppearanceSfx);
        }

        private void PlayWithRipple()
        {
            if(_useFlashAnimation)
                _appearanceImageFlash.TriggerFlash(OnAppearanceFinished);
            else
            {
                _maskAnimation.TriggerMask(OnAppearanceFinished);
            }
        }

        private void OnAppearanceFinished()
        {
            PlayRippleAnimation();
            if(_isNew)
                _newNotifierScaleAnimation.Play();
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