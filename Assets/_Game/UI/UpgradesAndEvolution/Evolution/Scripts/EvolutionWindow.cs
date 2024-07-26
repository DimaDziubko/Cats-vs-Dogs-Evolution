using System;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.UI._MainMenu.Scripts;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Screen = _Game.UI._MainMenu.Scripts.Screen;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionWindow : MonoBehaviour, IUIScreen
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private EvolutionTab _evolutionTab;
        [SerializeField] private TravelTab _travelTab;
        [SerializeField] private Button _timelineInfoButton;

        private IDisposable _disposable;

        private IEvolutionPresenter _evolutionPresenter;
        private ITimelineTravelPresenter _timelineTravelPresenter;
        private ITimelineInfoWindowProvider _timelineInfoProvider;
        private IAudioService _audioService;
        private IHeader _header;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        public Screen Screen => Screen.Evolution;


        public void Construct(
            IHeader header,
            IEvolutionPresenter evolutionPresenter,
            IAudioService audioService,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            ITimelineTravelPresenter timelineTravelPresenter,
            IUpgradesAvailabilityChecker upgradesChecker,
            IWorldCameraService cameraService)
        {
            _evolutionPresenter = evolutionPresenter;
            _timelineInfoProvider = timelineInfoWindowProvider;
            _timelineTravelPresenter = timelineTravelPresenter;
            _audioService = audioService;
            _header = header;
            _upgradesChecker = upgradesChecker;
            _evolutionTab.Construct(evolutionPresenter, audioService, timelineInfoWindowProvider);
            _travelTab.Construct(
                timelineTravelPresenter,
                audioService,
                timelineInfoWindowProvider,
                cameraService);
        }


        public void Show()
        {
            _header.ShowWindowName(Screen.ToString());
            
            _canvas.enabled = true;

            if (IsTimeToTravel())
            {
                _travelTab.Show();
            }
            else
            {
                _evolutionTab.Show();
            }

            Unsubscribe();
            Subscribe();

            _upgradesChecker.MarkAsReviewed(Screen);
            _upgradesChecker.MarkAsReviewed(Screen.UpgradesAndEvolution);
        }

        public void Hide()
        {
            _canvas.enabled = false;
            
            _travelTab.Hide();
            _evolutionTab.Hide();

            _disposable?.Dispose();

            Unsubscribe();
        }

        private void Subscribe()
        {
            _evolutionPresenter.LastAgeOpened += OnLastAgeOpened;
            _timelineInfoButton.onClick.AddListener(OnTimelineInfoBtnClicked);
        }

        private void Unsubscribe()
        {
            _evolutionPresenter.LastAgeOpened -= OnLastAgeOpened;
             _timelineInfoButton.onClick.RemoveAllListeners();
        }

        private void OnLastAgeOpened()
        {
            _travelTab.Show();
            _evolutionTab.Hide();
        }

        private bool IsTimeToTravel() => _timelineTravelPresenter.IsTimeToTravel();

        private async void OnTimelineInfoBtnClicked()
        {
            _audioService.PlayButtonSound();
            
            var window = await _timelineInfoProvider.Load();
            var isExited = await window.Value.ShowScreen();
            if(isExited) window.Dispose();
            _disposable = window;
        }
    }
}