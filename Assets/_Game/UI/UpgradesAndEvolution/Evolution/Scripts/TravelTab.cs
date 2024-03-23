using System;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class TravelTab : MonoBehaviour
    {
        public event Action TravelTabOpened;
    
        [SerializeField] private Canvas _canvas;
    
        [SerializeField] private TMP_Text _travelInfo;
    
        [SerializeField] private TransactionButton _travelButton;
        
        [SerializeField] private TMP_Text _travelConditionHint;

        private IEvolutionService _evolutionService;
        private IAudioService _audioService;
        
        public void Construct(
            IEvolutionService evolutionService,
            IAudioService audioService)
        {
            _audioService = audioService;
            _evolutionService = evolutionService;
        }
        
        public void Show()
        {
            Unsubscribe();
            Subscribe();
            
            _travelButton.Init();

            TravelTabOpened?.Invoke();
            
            _canvas.enabled = true;
        }

        private void Subscribe()
        {
            _travelButton.Click += OnTravelButtonClick;
            TravelTabOpened += _evolutionService.OnTravelTabOpened;
            _evolutionService.TravelViewModelUpdated += OnTravelViewModelUpdated;
        }

        public void Hide()
        {
            Unsubscribe();

            _travelButton.Cleanup();
            
            _canvas.enabled = false;
        }

        private void Unsubscribe()
        {
            TravelTabOpened -= _evolutionService.OnTravelTabOpened;
            _evolutionService.TravelViewModelUpdated -= OnTravelViewModelUpdated;
            _travelButton.Click -= OnTravelButtonClick;
        }

        private void OnTravelViewModelUpdated(TravelViewModel viewModel)
        {
            UpdateTravelConditionHint(viewModel.CanTravel);
            UpdateTravelInfoText(viewModel.NextTimelineNumber);
            UpdateTravelButton(viewModel.CanTravel);
        }

        private void UpdateTravelConditionHint(in bool viewModelCanTravel) => 
            _travelConditionHint.enabled = !viewModelCanTravel;

        private void UpdateTravelInfoText(int nextTimelineNumber) => 
            _travelInfo.text = $"Travel to timeline {nextTimelineNumber}";

        private void OnTravelButtonClick()
        {
            PlayButtonSound();
            _evolutionService.MoveToNextTimeline();
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();

        private void UpdateTravelButton(in bool canTravel) => 
            _travelButton.UpdateButtonState(canTravel, 0);
        
    }
}