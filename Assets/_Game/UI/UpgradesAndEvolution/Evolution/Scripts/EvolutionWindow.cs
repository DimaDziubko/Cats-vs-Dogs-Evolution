using System;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Pin.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionWindow : MonoBehaviour, IUIWindow
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private EvolutionTab _evolutionTab;
        [SerializeField] private TravelTab _travelTab;
        [SerializeField] private Button _timelineInfoButton;

        private IDisposable _disposable;

        private IEvolutionService _evolutionService;
        private ITimelineInfoWindowProvider _timelineInfoProvider;
        private IAudioService _audioService;
        private IHeader _header;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        public Window Window => Window.Evolution;


        public void Construct(
            IHeader header,
            IEvolutionService evolutionService,
            IAudioService audioService,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _evolutionService = evolutionService;
            _timelineInfoProvider = timelineInfoWindowProvider;
            _audioService = audioService;
            _header = header;
            _upgradesChecker = upgradesChecker;
            _evolutionTab.Construct(evolutionService, audioService, timelineInfoWindowProvider);
            _travelTab.Construct(evolutionService, audioService);
        }


        public void Show()
        {
            _header.ShowWindowName(Window.ToString());
            
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

            _upgradesChecker.MarkAsReviewed(Window);
            _upgradesChecker.MarkAsReviewed(Window.UpgradesAndEvolution);
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
            _evolutionService.LastAgeOpened += OnLastAgeOpened;
            _timelineInfoButton.onClick.AddListener(OnTimelineInfoBtnClicked);
        }

        private void Unsubscribe()
        {
            _evolutionService.LastAgeOpened -= OnLastAgeOpened;
             _timelineInfoButton.onClick.RemoveAllListeners();
        }

        private void OnLastAgeOpened()
        {
            _travelTab.Show();
            _evolutionTab.Hide();
        }

        private bool IsTimeToTravel() => _evolutionService.IsTimeToTravel();


        private async void OnTimelineInfoBtnClicked()
        {
            _audioService.PlayButtonSound();
            
            var window = await _timelineInfoProvider.Load();
            var isExited = await window.Value.AwaitForDecision(false);
            if(isExited) window.Dispose();
            _disposable = window;
        }
    }
}