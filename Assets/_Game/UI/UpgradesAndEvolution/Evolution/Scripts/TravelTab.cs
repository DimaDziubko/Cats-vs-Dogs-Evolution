using System;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI.Common.Scripts;
using _Game.UI.TimelineInfoScreen.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
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

        private ITimelineTravelPresenter _timelineTravelPresenter;
        private IAudioService _audioService;
        private ITimelineInfoScreenProvider _timelineInfoScreenProvider;
        private IWorldCameraService _cameraService;

        public void Construct(
            ITimelineTravelPresenter timelineTravelPresenter,
            IAudioService audioService,
            ITimelineInfoScreenProvider timelineInfoScreenProvider,
            IWorldCameraService cameraService)
        {
            _audioService = audioService;
            _timelineTravelPresenter = timelineTravelPresenter;
            _timelineInfoScreenProvider = timelineInfoScreenProvider;
            _cameraService = cameraService;
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
            TravelTabOpened += _timelineTravelPresenter.OnTravelTabOpened;
            _timelineTravelPresenter.TravelTabModelUpdated += OnTravelTabModelUpdated;
        }

        public void Hide()
        {
            Unsubscribe();

            _travelButton.Cleanup();
            
            _canvas.enabled = false;
        }

        private void Unsubscribe()
        {
            TravelTabOpened -= _timelineTravelPresenter.OnTravelTabOpened;
            _timelineTravelPresenter.TravelTabModelUpdated -= OnTravelTabModelUpdated;
            _travelButton.Click -= OnTravelButtonClick;
        }

        private void OnTravelTabModelUpdated(TravelTabModel tabModel)
        {
            UpdateTravelConditionHint(tabModel.CanTravel, tabModel.Hint);
            UpdateTravelInfoText(tabModel.NextTimelineNumber);
            UpdateTravelButton(tabModel.CanTravel);
        }

        private void UpdateTravelConditionHint(in bool modelCanTravel, string hint)
        {
            _travelConditionHint.text = hint;
            _travelConditionHint.enabled = !modelCanTravel;
        }

        private void UpdateTravelInfoText(int nextTimelineNumber) => 
            _travelInfo.text = $"Travel to timeline {nextTimelineNumber}";

        private async void OnTravelButtonClick()
        {
            PlayButtonSound();
            _timelineTravelPresenter.OpenNextTimeline();
            
            var travelAnimationScreenProvider = new TravelAnimationScreenProvider(_cameraService.UICameraOverlay);
            var window = await _timelineInfoScreenProvider.Load();
            var screen = await travelAnimationScreenProvider.Load();

            bool isAnimationCompleted = await screen.Value.Play();
            
            if (isAnimationCompleted)
            {
                screen.Dispose();
                var isExit = await window.Value.ShowScreenWithFistAgeAnimation();
                if (isExit)
                {
                    window.Dispose();
                }
            }
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();

        private void UpdateTravelButton(in bool canTravel)
        {
            var state = canTravel ? ButtonState.Active : ButtonState.Inactive;
            _travelButton.UpdateButtonState(state, "0", null, false);
        }
    }
}