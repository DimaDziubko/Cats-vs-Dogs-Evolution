using _Game.Core.DataPresenters.TimelineTravel;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.DataPresenters._UpgradeItemPresenter;
using Assets._Game.Core.DataPresenters.Evolution;
using Assets._Game.Core.DataPresenters.UnitUpgradePresenter;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI._MainMenu.Scripts;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
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
        [SerializeField] private TutorialStep _evolutionStep;
        
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
        private IMyLogger _logger;
        private ITutorialManager _tutorialManager;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IUpgradesAvailabilityChecker _upgradesChecker;

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IHeader header,
            IUpgradeItemPresenter upgradeItemPresenter,
            IEvolutionPresenter evolutionPresenter,
            ITimelineTravelPresenter timelineTravelPresenter,
            IUnitUpgradesPresenter unitUpgradesPresenter,
            IMyLogger logger,
            ITimelineInfoWindowProvider timelineInfoWindowProvider,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _logger = logger;

            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;
            
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _audioService = audioService;
            _upgradesChecker = upgradesChecker;
            
            _upgradesWindow.Construct(header, upgradeItemPresenter, unitUpgradesPresenter, audioService, tutorialManager, upgradesChecker);
            _evolutionWindow.Construct
                (header,
                evolutionPresenter,
                audioService, 
                timelineInfoWindowProvider, 
                timelineTravelPresenter, 
                upgradesChecker,
                cameraService);
            
        }

        public void Show()
        {
            _tutorialManager.Register(_evolutionStep);

            Unsubscribe();
            Subscribe();

            if (_featureUnlockSystem.IsFeatureUnlocked(_evolutionButton)) 
                _evolutionStep.ShowStep();

            if(_upgradesChecker.GetNotificationData(Window.Evolution).IsAvailable) 
                OnEvolutionButtonClick(_evolutionButton);
            else OnUpgradesButtonClick(_upgradesButton);
        }

        private void Subscribe()
        {
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            _upgradesChecker.Notify += OnUpgradeNotified;
            _upgradesButton.Initialize(
                true, 
                OnUpgradesButtonClick,
                PlayButtonSound, 
                _upgradesChecker.GetNotificationData(Window.Upgrades));
            _evolutionButton.Initialize(
                _featureUnlockSystem.IsFeatureUnlocked(_evolutionButton), 
                OnEvolutionButtonClick, 
                PlayButtonSound, 
                _upgradesChecker.GetNotificationData(Window.Evolution));
        }

        private void Unsubscribe()
        {
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            _upgradesChecker.Notify -= OnUpgradeNotified;
            _upgradesButton.Cleanup();
            _evolutionButton.Cleanup();
        }

        private void OnUpgradeNotified(NotificationData data)
        {
            
            switch (data.Window)
            {
                case Window.Upgrades:
                    _upgradesButton.SetupPin(data);
                    break;
                case Window.Evolution:
                    _evolutionButton.SetupPin(data);
                    break;
            }
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.EvolutionWindow)
            {
                _evolutionButton.Cleanup();
                _evolutionButton.Initialize(_featureUnlockSystem.IsFeatureUnlocked(_evolutionButton), 
                    OnEvolutionButtonClick, 
                    PlayButtonSound);
                _evolutionStep.ShowStep();
            }
        }

        private void OnUpgradesButtonClick(ToggleButton button)
        {
            ActiveButton = button;
            _evolutionWindow.Hide();
            _upgradesWindow.Show();
        }

        private void OnEvolutionButtonClick(ToggleButton button)
        {
            _evolutionStep.CompleteStep();
            ActiveButton = button;
            _evolutionWindow.Show();
            _upgradesWindow.Hide();
        }

        public void Hide()
        {
            _evolutionStep.CancelStep();
            _tutorialManager.UnRegister(_evolutionStep);

            Unsubscribe();
            
            _evolutionWindow.Hide();
            _upgradesWindow.Hide();
        }


        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
    }
}
