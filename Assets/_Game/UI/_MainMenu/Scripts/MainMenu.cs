using System.Collections;
using _Game.Core.GameState;
using _Game.UI._MainMenu.State;
using _Game.UI._Shop.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Utils.Disposable;
using UnityEngine;

namespace _Game.UI._MainMenu.Scripts
{
    public enum Screen
    {
        None,
        Battle,
        Upgrades,
        Evolution,
        UpgradesAndEvolution,
        Shop,
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
        
        [SerializeField] public TutorialStep _upgradesTutorialStep;

        private ToggleButton _activeButton;

        [SerializeField] private RectTransform[] _buttons;

        private MenuStateMachine _menuStateMachine;
        
        private IGameStateMachine _stateMachine;
        private IAudioService _audioService;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private ITutorialManager _tutorialManager;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        private IMyLogger _logger;


        //TODO Change step by step
        private bool IsDungeonUnlocked => false;
        private bool IsUpgradesUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_upgradeButton);
        private bool IsBattleUnlocked => true;
        private bool IsCardsUnlocked => false;
        private bool IsShopUnlocked => true;

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IStartBattleScreenProvider startBattleScreenProvider,
            IUpgradeAndEvolutionScreenProvider upgradeAndEvolutionScreenProvider,
            IShopProvider shopProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker,
            IMyLogger logger)
        {

            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
            _logger = logger;
            
            _menuStateMachine = new MenuStateMachine();
            BattleState battleState = new BattleState(this, startBattleScreenProvider, _battleButton);
            UpgradesState upgradesState = new UpgradesState(this, upgradeAndEvolutionScreenProvider, _upgradeButton);
            ShopState shopState = new ShopState(this, shopProvider, _shopButton);
            
            _menuStateMachine.AddState(battleState);
            _menuStateMachine.AddState(upgradesState);
            _menuStateMachine.AddState(shopState);
        }

        public void Show()
        {
            _tutorialManager.Register(_upgradesTutorialStep);

            Unsubscribe();
            Subscribe();

            _dungeonButton.Initialize(IsDungeonUnlocked, OnDungeonClick, PlayButtonSound);
            _upgradeButton.Initialize(IsUpgradesUnlocked, OnUpgradeButtonClick, PlayButtonSound,
                _upgradesChecker.GetNotificationData(Screen.UpgradesAndEvolution));
            _battleButton.Initialize(IsBattleUnlocked, OnBattleButtonClick, PlayButtonSound);
            _cardsButton.Initialize(IsCardsUnlocked, OnCardsButtonClick, PlayButtonSound);
            _shopButton.Initialize(IsShopUnlocked, OnShopButtonClick, PlayButtonSound);


            OnBattleButtonClick(_battleButton);

            if (IsUpgradesUnlocked)
            {
                StartCoroutine(ShowUpgradesTutorialStepWithDelay());
            }
        }

        public void SetActiveButton(ToggleButton button) => 
            _activeButton = button;

        private IEnumerator ShowUpgradesTutorialStepWithDelay()
        {
            yield return new WaitForSeconds(TUTORIAL_POINTER_DELAY);
            _upgradesTutorialStep.ShowStep();
        }
        
        private void Subscribe() => 
            _upgradesChecker.Notify += OnUpgradesNotified;

        private void OnUpgradesNotified(NotificationData data)
        {
            if (data.Screen == Screen.UpgradesAndEvolution)
            {
                _upgradeButton.SetupPin(data);
            }
        }

        private void Unsubscribe()
        {
            _upgradesChecker.Notify -= OnUpgradesNotified;
        }

        private void OnBattleButtonClick(ToggleButton button) => 
            _menuStateMachine.Enter<BattleState>();

        private void OnUpgradeButtonClick(ToggleButton button) => 
            _menuStateMachine.Enter<UpgradesState>();

        private void OnShopButtonClick(ToggleButton button) => 
            _menuStateMachine.Enter<ShopState>();

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
            
            _menuStateMachine.Cleanup();
            
            _upgradesTutorialStep.CancelStep();
            _tutorialManager.UnRegister(_upgradesTutorialStep);
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
        
        public void RebuildLayout()
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
