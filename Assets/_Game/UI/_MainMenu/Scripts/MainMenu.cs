using System;
using System.Collections;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.GameState;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using UnityEngine;

namespace _Game.UI._MainMenu.Scripts
{
    public enum Window
    {
        None,
        Battle,
        Upgrades,
        Evolution,
        UpgradesAndEvolution,
    }

    [RequireComponent(typeof(Canvas))]
    public class MainMenu : MonoBehaviour
    {
        private static float TUTORIAL_POINTER_DELAY = 2F;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _canvasRectTransform;
        
        [SerializeField] private ToggleButton _dungeonButton;
        [SerializeField] private ToggleButton _upgradeButton;
        [SerializeField] private ToggleButton _battleButton;
        [SerializeField] private ToggleButton _cardsButton;
        [SerializeField] private ToggleButton _shopButton;

        [SerializeField] private float _highlightedBtnScale = 1.65f;
        
        [SerializeField] private TutorialStep _upgradesTutorialStep;

        private ToggleButton _activeButton;

        [SerializeField] private RectTransform[] _buttons;

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

        
        private IGameStateMachine _stateMachine;
        private IShopPopupProvider _shopPopupProvider;
        private IAudioService _audioService;
        private IStartBattleWindowProvider _startBattleWindowProvider;
        private IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private ITutorialManager _tutorialManager;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        private IMyLogger _logger;


        //TODO Change step by step
        private bool IsDungeonUnlocked => false;
        private bool IsUpgradesUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_upgradeButton);
        private bool IsBattleUnlocked => true;
        private bool IsCardsUnlocked => false;
        private bool IsShopUnlocked => false;

        private Window _window = Window.None;

        private Disposable<StartBattleWindow> _startBattleWindow;
        private Disposable<UpgradeAndEvolutionWindow> _upgradeAndEvolutionWindow;


        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IStartBattleWindowProvider startBattleWindowProvider,
            IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker,
            IMyLogger logger)
        {

            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _startBattleWindowProvider = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
            _logger = logger;
        }

        public void Show()
        {
            _tutorialManager.Register(_upgradesTutorialStep);

            Unsubscribe();
            Subscribe();

            _dungeonButton.Initialize(IsDungeonUnlocked, OnDungeonClick, PlayButtonSound);
            _upgradeButton.Initialize(IsUpgradesUnlocked, OnUpgradeButtonClick, PlayButtonSound,
                _upgradesChecker.GetNotificationData(Window.UpgradesAndEvolution));
            _battleButton.Initialize(IsBattleUnlocked, OnBattleButtonClick, PlayButtonSound);
            _cardsButton.Initialize(IsCardsUnlocked, OnCardsButtonClick, PlayButtonSound);
            _shopButton.Initialize(IsCardsUnlocked, OnShopButtonClick, PlayButtonSound);


            OnBattleButtonClick(_battleButton);

            if (IsUpgradesUnlocked)
            {
                StartCoroutine(ShowUpgradesTutorialStepWithDelay());
            }
        }

        private IEnumerator ShowUpgradesTutorialStepWithDelay()
        {
            yield return new WaitForSeconds(TUTORIAL_POINTER_DELAY);
            _upgradesTutorialStep.ShowStep();
        }
        
        private void Subscribe()
        {
            _upgradesChecker.Notify += OnUpgradesNotified;
        }

        private void OnUpgradesNotified(NotificationData data)
        {
            if (data.Window == Window.UpgradesAndEvolution)
            {
                _upgradeButton.SetupPin(data);
            }
        }

        private void Unsubscribe()
        {
            _upgradesChecker.Notify -= OnUpgradesNotified;
        }

        private void OnShopButtonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        private void OnCardsButtonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        private void OnDungeonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        public void Hide()
        {
            Unsubscribe();
            _upgradeButton.Cleanup();
            _battleButton.Cleanup();
            _dungeonButton.Cleanup();
            _cardsButton.Cleanup();
            _shopButton.Cleanup();

            if (_startBattleWindow != null)
            {
                _startBattleWindow.Value.Hide();
                _startBattleWindow?.Dispose();
            }

            if (_upgradeAndEvolutionWindow != null)
            {
                _upgradeAndEvolutionWindow.Value.Hide();
                _upgradeAndEvolutionWindow?.Dispose();
            }

            _upgradesTutorialStep.CancelStep();
            _tutorialManager.UnRegister(_upgradesTutorialStep);
        }

        private async void OnBattleButtonClick(ToggleButton button)
        {
            if (_window != Window.Battle)
            {
                _startBattleWindow
                    = await _startBattleWindowProvider.Load();
                _window = Window.Battle;
                _startBattleWindow.Value.Show();

                ActiveButton = button;

                if (_upgradeAndEvolutionWindow != null)
                {
                    _upgradeAndEvolutionWindow.Value.Hide();
                    _upgradeAndEvolutionWindow?.Dispose();
                    _upgradeAndEvolutionWindow = null;
                }
            }
            
            RebuildLayout();
        }

        private async void OnUpgradeButtonClick(ToggleButton button)
        {
            if (_window != Window.UpgradesAndEvolution)
            {
                _upgradesTutorialStep.CompleteStep();

                _upgradeAndEvolutionWindow
                    = await _upgradeAndEvolutionWindowProvider.Load();

                _upgradeAndEvolutionWindow.Value.Show();
                _window = Window.UpgradesAndEvolution;

                ActiveButton = button;

                if (_startBattleWindow != null)
                {
                    _startBattleWindow.Value.Hide();
                    _startBattleWindow?.Dispose();
                    _startBattleWindow = null;
                }
            }

            RebuildLayout();
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
        
        private void RebuildLayout()
        {
            var with = _canvasRectTransform.rect.width;
            float totalButtonWidth = with / (_buttons.Length - 1 + _highlightedBtnScale);

            float buttonWidth = totalButtonWidth;
            float activeButtonWidth = totalButtonWidth * _highlightedBtnScale;

            float xPosition = - with / 2;

            foreach (RectTransform button in _buttons)
            {
                if (button == _activeButton.RectTransform)
                {
                    button.sizeDelta = new Vector2(activeButtonWidth, button.sizeDelta.y);
                    button.anchoredPosition = new Vector2(xPosition + activeButtonWidth / 2, button.anchoredPosition.y);
                    xPosition += activeButtonWidth;
                }
                else
                {
                    button.sizeDelta = new Vector2(buttonWidth, button.sizeDelta.y);
                    button.anchoredPosition = new Vector2(xPosition + buttonWidth / 2, button.anchoredPosition.y);
                    xPosition += buttonWidth;
                }
            }
        }
    }
}
