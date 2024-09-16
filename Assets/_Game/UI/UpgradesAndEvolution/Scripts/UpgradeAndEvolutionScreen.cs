using _Game.Core._DataPresenters._UpgradeItemPresenter;
using _Game.Core._DataPresenters.UnitUpgradePresenter;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.DataPresenters.Evolution;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Header.Scripts;
using _Game.UI.TimelineInfoScreen.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Gameplay._Tutorial.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradesScreen upgradesScreen;
        [SerializeField] private EvolutionScreen evolutionScreen;
        [SerializeField] private ToggleButton _upgradesButton;
        [SerializeField] private ToggleButton _evolutionButton;
        [SerializeField] private TutorialStep _evolutionStep;
        [SerializeField] private QuickBoostInfoPanel _quickBoostInfoPanel;
        
        private ToggleButton _activeButton;
        
        private ToggleButton ActiveButton
        {
            get => _activeButton;
            set
            {
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
            ITimelineInfoScreenProvider timelineInfoScreenProvider,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IUpgradesAvailabilityChecker upgradesChecker, 
            IMiniShopProvider miniShopProvider,
            IBoostDataPresenter boostDataPresenter)
        {
            _logger = logger;

            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;

            _canvas.worldCamera = cameraService.UICameraOverlay;
            _audioService = audioService;
            _upgradesChecker = upgradesChecker;

            _quickBoostInfoPanel.Construct(
                boostDataPresenter,
                audioService,
                BoostSource.TotalBoosts);
            
            upgradesScreen.Construct(
                header, 
                upgradeItemPresenter, 
                unitUpgradesPresenter, 
                audioService, 
                tutorialManager, 
                upgradesChecker,
                miniShopProvider);
            evolutionScreen.Construct
                (header,
                evolutionPresenter,
                audioService,
                timelineInfoScreenProvider,
                timelineTravelPresenter,
                upgradesChecker,
                cameraService,
                miniShopProvider);

        } 

        public void Show()
        {
            _tutorialManager.Register(_evolutionStep);

            Unsubscribe();
            Subscribe();

            if (_featureUnlockSystem.IsFeatureUnlocked(_evolutionButton))
                _evolutionStep.ShowStep();

            if (_upgradesChecker.GetNotificationData(GameScreen.Evolution).IsAvailable)
                OnEvolutionButtonClick(_evolutionButton);
            else OnUpgradesButtonClick(_upgradesButton);
            
            _quickBoostInfoPanel.Init();
        }
        
        private void Subscribe()
        {
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            _upgradesChecker.Notify += OnUpgradeNotified;
            _upgradesButton.Initialize(
                true,
                OnUpgradesButtonClick,
                PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.Upgrades));
            _evolutionButton.Initialize(
                _featureUnlockSystem.IsFeatureUnlocked(_evolutionButton),
                OnEvolutionButtonClick,
                PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.Evolution));
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
            switch (data.GameScreen)
            {
                case GameScreen.Upgrades:
                    _upgradesButton.SetupPin(data);
                    break;
                case GameScreen.Evolution:
                    _evolutionButton.SetupPin(data);
                    break;
            }
        }


        private void UpdateButtonsView(GameScreen gameScreen)
        {
            if (_activeButton != null)
            {
                _activeButton.UnHighlightBtn();
            }
            
            switch (gameScreen)
            {
                case GameScreen.Upgrades:
                    MoveUpInHierarchy(_upgradesButton.RectTransform);
                    break;
                case GameScreen.Evolution:
                    MoveUpInHierarchy(_evolutionButton.RectTransform);
                    break;
            }
        }

        private void MoveUpInHierarchy(RectTransform rectTransform)
        {
            int siblingIndex = rectTransform.GetSiblingIndex();
            int siblingCount = rectTransform.parent.childCount;

            if (siblingIndex < siblingCount - 1)
            {
                rectTransform.SetSiblingIndex(siblingIndex + 1);
            }
        }
        
        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.EvolutionScreen)
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
            evolutionScreen.Hide();
            UpdateButtonsView(GameScreen.Upgrades);
            ActiveButton = button;
            upgradesScreen.Show();
        }

        private void OnEvolutionButtonClick(ToggleButton button)
        {
            _evolutionStep.CompleteStep();
            evolutionScreen.Show();
            UpdateButtonsView(GameScreen.Evolution);
            ActiveButton = button;
            upgradesScreen.Hide();
        }

        public void Hide()
        {
            _evolutionStep.CancelStep();
            _tutorialManager.UnRegister(_evolutionStep);

            Unsubscribe();

            evolutionScreen.Hide();
            upgradesScreen.Hide();
            
            _quickBoostInfoPanel.Cleanup();
        }


        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();

    }
}
