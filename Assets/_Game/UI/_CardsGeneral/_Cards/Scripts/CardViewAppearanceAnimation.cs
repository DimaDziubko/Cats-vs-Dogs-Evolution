using _Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
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

        private Tween _bgScaleTween;
        private Tween _rippleScaleTween;

        public void Init(IAudioService audioService, Color flashColor, bool isNew)
        {
            _audioService = audioService;
            _appearanceImageFlash.Init();
            _rippleImageAlphaFlash.Init(flashColor);
            _maskAnimation.Init();
            _newNotifierScaleAnimation.Init();
            _isNew = isNew;
        }

        public async UniTask PlayAsync(bool needIlluminationAnimation)
        {
            if (!needIlluminationAnimation)
            {
                await PlaySimpleAsync();
            }
            else
            {
                await PlayWithRippleAsync();
            }
        }

        private async UniTask PlaySimpleAsync()
        {
            if (_useFlashAnimation)
                await _appearanceImageFlash.TriggerFlashAsync();
            else
                await _maskAnimation.TriggerMaskAsync();

            _audioService.PlayOneShot(_cardAppearanceSfx);
        }

        private async UniTask PlayWithRippleAsync()
        {
            if (_useFlashAnimation)
                await _appearanceImageFlash.TriggerFlashAsync();
            else
                await _maskAnimation.TriggerMaskAsync();

            await OnAppearanceFinishedAsync();
        }

        private async UniTask OnAppearanceFinishedAsync()
        {
            _newNotifierScaleAnimation.Cleanup();
            
            _audioService.PlayOneShot(_cardIlluminationSfx);
            
            var rippleTask = PlayRippleAnimationAsync();
            UniTask notifierTask = UniTask.CompletedTask;

            if (_isNew)
            {
                notifierTask = _newNotifierScaleAnimation.PlayAsync();
            }
            
            await UniTask.WhenAll(rippleTask, notifierTask);
        }

        private async UniTask PlayRippleAnimationAsync()
        {
            Cleanup();
            
            _bgScaleTween = _cardBgTransform.DOScale(_newCardScale, _scaleAnimationDuration / 2)
                .OnComplete(() => _bgScaleTween = _cardBgTransform.DOScale(1, _scaleAnimationDuration / 2));

            _rippleImage.enabled = true;
            _rippleImageAlphaFlash.TriggerFlash();
            
            _rippleScaleTween = _rippleTransform.DOScale(_rippleScale, _rippleAnimationDuration)
                .OnComplete(() => _rippleImage.enabled = false);

            await UniTask.Delay((int) (_rippleAnimationDuration * 1000));
        }

        public void Cleanup()
        {
            if (_bgScaleTween?.IsActive() == true) _bgScaleTween.Kill();
            if (_rippleScaleTween?.IsActive() == true) _rippleScaleTween.Kill();
        }
    }
}