using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Popups;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionWindow : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradesWindow _upgradesWindow;
        [SerializeField] private EvolutionWindow _evolutionWindow;
        
        [SerializeField] private ToggleButton _upgradesButton;
        [SerializeField] private ToggleButton _evolutionButton;
        
        private ToggleButton _activeButton;
        
        private ToggleButton ActiveButton
        {
            get => _activeButton;
            set 
            {
                if (_activeButton != null)
                {
                    _activeButton.UnHighlightBtn();
                }
                _activeButton = value;
                _activeButton.HighlightBtn();
            }
        }
        
        private IAudioService _audioService;
        private IAlertPopupProvider _alertPopupProvider;
        private IMyLogger _logger;
        
        public void Construct(Camera uICamera,
            IAudioService audioService,
            IAlertPopupProvider alertPopupProvider,
            IHeader header,
            IEconomyUpgradesService economyUpgradesService,
            IEvolutionService evolutionService,
            IUnitUpgradesService unitUpgradesService,
            IMyLogger logger,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            ITutorialManager tutorialManager)
        {
            _logger = logger;
            
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _alertPopupProvider = alertPopupProvider;

            _upgradesWindow.Construct(header, economyUpgradesService, unitUpgradesService, audioService, tutorialManager);
            _evolutionWindow.Construct(header, evolutionService, audioService, timelineInfoWindowProvider);
            
        }

        public void Show()
        {
            //TODO Delete later
            _logger.Log("UpgradeAndEvolutionWindow SHOW");
            
            Unsubscribe();
            Subscribe();

            OnUpgradesButtonClick(_upgradesButton);
        }

        private void Subscribe()
        {
            _upgradesButton.Initialize(false, OnUpgradesButtonClick, PlayButtonSound);
            _evolutionButton.Initialize(false, OnEvolutionButtonClick, PlayButtonSound);
        }

        private void OnUpgradesButtonClick(ToggleButton button)
        {
            ActiveButton = button;
            _evolutionWindow.Hide();
            _upgradesWindow.Show();
        }

        public void Hide()
        {
            //TODO Delete later
            _logger.Log("UpgradeAndEvolutionWindow HIDE");
            
            Unsubscribe();

            _evolutionWindow.Hide();
            _upgradesWindow.Hide();
        }

        private void Unsubscribe()
        {
            _upgradesButton.Cleanup();
            _evolutionButton.Cleanup();
        }

        private void OnEvolutionButtonClick(ToggleButton button)
        {
            ActiveButton = button;
            _evolutionWindow.Show();
            _upgradesWindow.Hide();
        }
        
        
        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
    }
}
