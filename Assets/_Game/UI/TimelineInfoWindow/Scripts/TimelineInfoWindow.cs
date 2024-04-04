using System;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
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
        private IEvolutionService _evolutionService;
        private IMyLogger _logger;


        //Animation data
        private int _currentAge;
        private int _ages;

        public void Construct(
            IAudioService audioService, 
            IEvolutionService evolutionService, 
            IMyLogger logger)
        {
            _audioService = audioService;
            _evolutionService = evolutionService;
            _logger = logger;
        }

        public async UniTask<bool> AwaitForDecision(bool needAnimation)
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            Show();
            
            if(needAnimation) PlayEvolveAnimation();
            
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void PlayEvolveAnimation()
        {
            _exitBtn.gameObject.SetActive(false);
            
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
                    _evolutionService.MoveToNextAge();
                    _exitBtn.gameObject.SetActive(true);
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
            Opened += _evolutionService.OnTimelineInfoWindowOpened;
            _evolutionService.TimelineInfoDataUpdated += UpdateUIElements;
            _exitBtn.onClick.AddListener(OnExit);
        }

        private void Unsubscribe()
        {
            Opened -= _evolutionService.OnTimelineInfoWindowOpened;
            _evolutionService.TimelineInfoDataUpdated -= UpdateUIElements;
            _exitBtn.onClick.RemoveAllListeners();
        }

        private void UpdateUIElements(TimelineInfoData data)
        {
            _currentAge = data.CurrentAge;
            _ages = data.Models.Count;
            
            UpdateItems(data);
            UpdateSlider(data.CurrentAge, data.Models.Count);
            AdjustScrollPosition(data.CurrentAge, data.Models.Count);
        }

        private void UpdateSlider(int currentAge, int ages)
        {
            _progressBar.UpdateValue(currentAge, ages);
        }

        private void UpdateItems(TimelineInfoData data)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Initialize(data.Models[i]);
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
