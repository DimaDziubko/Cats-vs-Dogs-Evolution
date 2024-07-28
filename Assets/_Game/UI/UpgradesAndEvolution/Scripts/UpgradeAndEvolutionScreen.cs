using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.DataPresenters.TimelineTravel;
using _Game.UI._MainMenu.Scripts;
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
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;
using Screen = _Game.UI._MainMenu.Scripts.Screen;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradesWindow _upgradesWindow;
        [SerializeField] private EvolutionWindow _evolutionWindow;
        [SerializeField] private ToggleButton _upgradesButton;
        [SerializeField] private ToggleButton _evolutionButton;
        [SerializeField] private TutorialStep _evolutionStep;

        private ToggleButton _activeButton;

        private Vector2 _upgradeBtnStartSize;
        private Vector2 _upgradeBtnStartPos;

        private Vector2 _evolutionBtnStartSize;
        private Vector2 _evolutionBtnStartPos;
        private float _expansionAmount = 25f;

        private ToggleButton ActiveButton
        {
            get => _activeButton;
            set
            {
                //if (_activeButton != null)
                //{
                //    _activeButton.UnHighlightBtn();
                //}
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
        private void Awake()
        {
            _upgradeBtnStartSize = _upgradesButton.RectTransform.sizeDelta;
            _upgradeBtnStartPos = _upgradesButton.RectTransform.anchoredPosition;

            _evolutionBtnStartSize = _evolutionButton.RectTransform.sizeDelta;
            _evolutionBtnStartPos = _evolutionButton.RectTransform.anchoredPosition;
        }

        public void Show()
        {
            _tutorialManager.Register(_evolutionStep);

            Unsubscribe();
            Subscribe();

            if (_featureUnlockSystem.IsFeatureUnlocked(_evolutionButton))
                _evolutionStep.ShowStep();

            if (_upgradesChecker.GetNotificationData(Screen.Evolution).IsAvailable)
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
                _upgradesChecker.GetNotificationData(Screen.Upgrades));
            _evolutionButton.Initialize(
                _featureUnlockSystem.IsFeatureUnlocked(_evolutionButton),
                OnEvolutionButtonClick,
                PlayButtonSound,
                _upgradesChecker.GetNotificationData(Screen.Evolution));
        }

        private void Unsubscribe()
        {
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            _upgradesChecker.Notify -= OnUpgradeNotified;
            _upgradesButton.Cleanup();
            _evolutionButton.Cleanup();
        }

        //ExpandRightSide(_upgradesButton.RectTransform, 50f);

        private void OnUpgradeNotified(NotificationData data)
        {
            switch (data.Screen)
            {
                case Screen.Upgrades:
                    _upgradesButton.SetupPin(data);
                    break;
                case Screen.Evolution:
                    _evolutionButton.SetupPin(data);
                    break;
            }
        }


        private void UpdateButtonsView(Screen screen)
        {
            if (_activeButton != null)
            {
                _activeButton.UnHighlightBtn();
            }

            _upgradesButton.RectTransform.sizeDelta = _upgradeBtnStartSize;
            _upgradesButton.RectTransform.anchoredPosition = _upgradeBtnStartPos;

            _evolutionButton.RectTransform.sizeDelta = _evolutionBtnStartSize;
            _evolutionButton.RectTransform.anchoredPosition = _evolutionBtnStartPos;


            switch (screen)
            {
                case Screen.Upgrades:
                    ExpandLeftSide(_evolutionButton.RectTransform, _expansionAmount);
                    MoveUpInHierarchy(_upgradesButton.RectTransform);
                    break;
                case Screen.Evolution:
                    ExpandRightSide(_upgradesButton.RectTransform, _expansionAmount);
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
        private void ExpandRightSide(RectTransform rectTransform, float amount)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + amount, rectTransform.sizeDelta.y);

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + amount / 2, rectTransform.anchoredPosition.y);
        }
        private void ExpandLeftSide(RectTransform rectTransform, float amount)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + amount, rectTransform.sizeDelta.y);

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - amount / 2, rectTransform.anchoredPosition.y);
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
            _evolutionWindow.Hide();
            UpdateButtonsView(Screen.Upgrades);
            ActiveButton = button;
            _upgradesWindow.Show();
        }

        private void OnEvolutionButtonClick(ToggleButton button)
        {
            _evolutionStep.CompleteStep();
            _evolutionWindow.Show();
            UpdateButtonsView(Screen.Evolution);
            ActiveButton = button;
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
