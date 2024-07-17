using System;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using Assets._Game.Core._Logger;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.TimelineInfoWindow.Scripts
{
    public class TimelineInfoWindow : MonoBehaviour
    {
        public event Action Opened;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private TimelineInfoItem[] _items;
        [SerializeField] private TimelineProgressBar _progressBar;
        [SerializeField] private Button _exitBtn;
        [SerializeField] private AudioClip _evolveSFX;

        [SerializeField] private float _animationDelay = 1.0f;
        [SerializeField] private float _scrollAnimationDuration = 3f;
        [SerializeField] private float _rippleAnimationDuration = 0.2f;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private ITimelineInfoPresenter _timelineInfoPresenter;
        private IEvolutionPresenter _evolutionPresenter;
        private IMyLogger _logger;

        //Animation data
        private int _currentAge;
        private int _ages;

        public void Construct(
            IAudioService audioService,
            ITimelineInfoPresenter evolutionService,
            IEvolutionPresenter evolutionPresenter,
            IMyLogger logger,
            IWorldCameraService cameraService)
        {
            _audioService = audioService;
            _timelineInfoPresenter = evolutionService;
            _evolutionPresenter = evolutionPresenter;
            _logger = logger;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            Show();
            _exitBtn.interactable = false;
        }


        public async UniTask<bool> ShowScreen()
        {
            _canvas.enabled = true;
            _exitBtn.interactable = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            Show();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        public async UniTask<bool> ShowScreenWithTransitionAnimation()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            Show();
            PlayEvolveAnimation();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        public async UniTask<bool> ShowScreenWithFistAgeAnimation()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            Show();
            PlayFirstAgeAnimation();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void PlayFirstAgeAnimation()
        {
            _exitBtn.interactable = false;
            Sequence preAnimationSequence = DOTween.Sequence().AppendInterval(_animationDelay);
            preAnimationSequence.OnComplete(() =>
            {
                Sequence sequence = DOTween.Sequence();

                int age = 0;
                sequence.AppendCallback(() =>
                {
                    _items[age].PlayRippleAnimation(_rippleAnimationDuration);
                    _progressBar.PlayMarkerRippleAnimation(age, _rippleAnimationDuration);
                });

                sequence.OnComplete(() =>
                {
                    _exitBtn.interactable = true;
                });
            });
        }

        private void PlayEvolveAnimation()
        {
            _exitBtn.interactable = false;

            PlayEvolveSound();

            Sequence preAnimationSequence = DOTween.Sequence().AppendInterval(_animationDelay);

            preAnimationSequence.OnComplete(() =>
            {
                Sequence sequence = DOTween.Sequence();

                int nextAge = _currentAge + 1;
                float nextViewportAndBarValue = (float)nextAge / (_ages - 1);

                sequence.Append(_scrollRect.DOHorizontalNormalizedPos(nextViewportAndBarValue, _scrollAnimationDuration));
                sequence.Join(_progressBar.PlayValueAnimation(nextViewportAndBarValue, _scrollAnimationDuration));

                sequence.AppendCallback(() =>
                {
                    _items[nextAge].PlayRippleAnimation(_rippleAnimationDuration);
                    _progressBar.PlayMarkerRippleAnimation(nextAge, _rippleAnimationDuration);
                });

                sequence.OnComplete(() =>
                {
                    _evolutionPresenter.OpenNextAge();
                    _exitBtn.interactable = true;
                });
            });
        }

        private void AdjustScrollPosition(int currentItem, int items)
        {
            float scrollPercentage = ((float)currentItem / (items - 1));
            _scrollRect.horizontalNormalizedPosition = scrollPercentage;
        }

        private void Show()
        {
            Unsubscribe();
            Subscribe();

            Opened?.Invoke();
        }


        private void OnExit()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
            Unsubscribe();
        }

        private void Subscribe()
        {
            Opened += _timelineInfoPresenter.OnPrepareTimelineInfoDataForShow;
            Opened += _timelineInfoPresenter.OnTimelineInfoWindowOpened;

            _timelineInfoPresenter.TimelineInfoDataUpdated += UpdateUIElements;
            _exitBtn.onClick.AddListener(OnExit);
        }

        private void Unsubscribe()
        {
            Opened -= _timelineInfoPresenter.OnPrepareTimelineInfoDataForShow;
            Opened -= _timelineInfoPresenter.OnTimelineInfoWindowOpened;

            _timelineInfoPresenter.TimelineInfoDataUpdated -= UpdateUIElements;
            _exitBtn.onClick.RemoveAllListeners();
        }

        private void UpdateUIElements(TimelineInfoModel model)
        {
            Debug.Log("UpdateUIElements");

            _currentAge = model.CurrentAge;
            _ages = model.Models.Count;

            UpdateItems(model);
            UpdateSlider(model.CurrentAge, model.Models.Count);
            AdjustScrollPosition(model.CurrentAge, model.Models.Count);
        }

        private void UpdateSlider(int currentAge, int ages) =>
            _progressBar.UpdateValue(currentAge, ages);

        private void UpdateItems(TimelineInfoModel model)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].UpdateModel(model.Models[i]);
            }
        }

        private void PlayEvolveSound()
        {
            if (_audioService != null && _evolveSFX != null)
            {
                _audioService.PlayOneShot(_evolveSFX);
            }
        }
    }
}
