using System;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionTab : MonoBehaviour
    {
        public event Action EvolutionTabOpened;
    
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Image _currentAgeImage;
        [SerializeField] private TMP_Text _currentAgeName;
        
        [SerializeField] private Image _nextAgeImage;
        [SerializeField] private TMP_Text _nextAgeName;
    
        [SerializeField] private TransactionButton _evolveButton;

        [SerializeField] private TMP_Text _timelineLabel;
    
        private IDisposable _disposable;
        
        private IEvolutionService _evolutionService;
        private IHeader _header;
        private IAudioService _audioService;
        private ITimelineInfoWindowProvider _timelineInfoProvider;

        public void Construct(
            IEvolutionService evolutionService,
            IAudioService audioService,
            ITimelineInfoWindowProvider timelineInfoProvider)
        {
            _audioService = audioService;
            _evolutionService = evolutionService;
            _timelineInfoProvider = timelineInfoProvider;
        }
    
        public void Show()
        {
            _evolveButton.Init();
            
            Unsubscribe();

            Subscribe();

            EvolutionTabOpened?.Invoke();
            
            _canvas.enabled = true;
        }

        private void Subscribe()
        {
            _evolveButton.Click += OnEvolveButtonClick;
            EvolutionTabOpened += _evolutionService.OnEvolutionTabOpened;
            _evolutionService.EvolutionViewModelUpdated += OnEvolutionViewModelUpdated;
        }

        private void Unsubscribe()
        {
            _evolveButton.Click -= OnEvolveButtonClick;
            EvolutionTabOpened -= _evolutionService.OnEvolutionTabOpened;
            _evolutionService.EvolutionViewModelUpdated -= OnEvolutionViewModelUpdated;
        }

        public void Hide()
        {
            Unsubscribe();
            _evolveButton.Cleanup();
            _disposable?.Dispose();
            _canvas.enabled = false;
        }

        private void OnEvolutionViewModelUpdated(EvolutionViewModel viewModel)
        {
            UpdateTimelineLabel(viewModel.CurrentTimelineId);
            UpdateButtonData(viewModel.EvolutionBtnData);

            _currentAgeImage.sprite = viewModel.CurrentAgeIcon;
            _nextAgeImage.sprite = viewModel.NextAgeIcon;
            _currentAgeName.text = viewModel.CurrentAgeName;
            _nextAgeName.text = viewModel.NextAgeName;
        }

        private void UpdateButtonData(EvolutionBtnData data) => 
            _evolveButton.UpdateButtonState(data.CanAfford, data.Price);

        private async void OnEvolveButtonClick()
        {
            PlayButtonSound();
            
            var window = await _timelineInfoProvider.Load();
            var isExited = await window.Value.AwaitForDecision(true);
            
            if(isExited) window.Dispose();
            _disposable = window;
            
            //TODO Play animation here then evolve
            //_evolutionService.MoveToNextAge();
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();


        private void UpdateTimelineLabel(int id) => 
            _timelineLabel.text = $"Timeline {id + 1}";
        
    }
}