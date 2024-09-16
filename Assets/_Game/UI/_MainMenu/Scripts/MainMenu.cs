using System.Collections;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.GameState;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Temp;
using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._Hud;
using _Game.UI._MainMenu.State;
using _Game.UI._Shop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using UnityEngine;

namespace _Game.UI._MainMenu.Scripts
{
    public enum GameScreen
    {
        None,
        Battle,
        Upgrades,
        Evolution,
        UpgradesAndEvolution,
        Shop,
        GeneralCards,
        Cards,
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
        [SerializeField] private TutorialStep _cardsTutorialStep;

        public TutorialStep UpgradeTutorialStep => _upgradesTutorialStep;
        public TutorialStep CardsTutorialStep => _cardsTutorialStep;
        
        
        private ToggleButton _activeButton;

        [SerializeField] private RectTransform[] _buttons;

        private LocalStateMachine _menuStateMachine;

        private IGameStateMachine _stateMachine;
        private IAudioService _audioService;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private ITutorialManager _tutorialManager;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        private IMyLogger _logger;
        private Curtain _curtain;

        //TODO Change step by step
        private bool IsDungeonUnlocked => false;
        private bool IsUpgradesUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_upgradeButton);
        private bool IsBattleUnlocked => true;
        private bool IsCardsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_cardsButton);
        private bool IsShopUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_shopButton);

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IStartBattleScreenProvider startBattleScreenProvider,
            IUpgradeAndEvolutionScreenProvider upgradeAndEvolutionScreenProvider,
            IShopProvider shopProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker,
            IMyLogger logger,
            Curtain curtain,
            IUINotifier uiNotifier,
            IGeneralCardsScreenProvider generalCardsScreenProvider)
        {

            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
            _logger = logger;
            _curtain = curtain;

            _menuStateMachine = new LocalStateMachine();
            BattleState battleState = new BattleState(this, startBattleScreenProvider, _battleButton, uiNotifier);
            UpgradesState upgradesState = new UpgradesState(this, upgradeAndEvolutionScreenProvider, _upgradeButton, uiNotifier);
            ShopState shopState = new ShopState(this, shopProvider, _shopButton, uiNotifier);
            GeneralCardsState generalCardsState = new GeneralCardsState(this, generalCardsScreenProvider, _cardsButton, uiNotifier);
            
            _menuStateMachine.AddState(battleState);
            _menuStateMachine.AddState(upgradesState);
            _menuStateMachine.AddState(shopState);
            _menuStateMachine.AddState(generalCardsState);
        }

        public void HideCurtain() => _curtain.Hide();

        public void ShowCurtain() => _curtain.Show();

        public void Show()
        {
            _tutorialManager.Register(UpgradeTutorialStep);
            _tutorialManager.Register(CardsTutorialStep);

            Unsubscribe();
            Subscribe();

            _dungeonButton.Initialize(IsDungeonUnlocked, OnDungeonClick, PlayButtonSound);
            _upgradeButton.Initialize(IsUpgradesUnlocked, OnUpgradeButtonClick, PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.UpgradesAndEvolution));
            _battleButton.Initialize(IsBattleUnlocked, OnBattleButtonClick, PlayButtonSound);
            _cardsButton.Initialize(IsCardsUnlocked, OnCardsButtonClick, PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.GeneralCards));
            _shopButton.Initialize(IsShopUnlocked, OnShopButtonClick, PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.Shop));


            OnBattleButtonClick(_battleButton);

            ShowUpgradeTutorialWithDelay(TUTORIAL_POINTER_DELAY);
            ShowCardsTutorialWithDelay(TUTORIAL_POINTER_DELAY);
        }

        public void ShowCardsTutorialWithDelay(float delay)
        {
            if (IsCardsUnlocked)
            {
                StartCoroutine(ShowTutorialStepWithDelay(CardsTutorialStep, delay));
            }
        }

        public void ShowUpgradeTutorialWithDelay(float delay)
        {
            if (IsUpgradesUnlocked)
            {
                StartCoroutine(ShowTutorialStepWithDelay(UpgradeTutorialStep, delay));
            }
        }

        public void SetActiveButton(ToggleButton button) => 
            _activeButton = button;

        private IEnumerator ShowTutorialStepWithDelay(TutorialStep tutorialStep, float delay)
        {
            yield return new WaitForSeconds(delay);
            tutorialStep.ShowStep();
        }

        private void Subscribe()
        {
            _upgradesChecker.Notify += OnUpgradesNotified;
            GlobalEvents.OnInsufficientFunds += OnInsufficientFunds;
        }

        private void OnUpgradesNotified(NotificationData data)
        {
            switch (data.GameScreen)
            {
                case GameScreen.None:
                    break;
                case GameScreen.Battle:
                    break;
                case GameScreen.Upgrades:
                    break;
                case GameScreen.Evolution:
                    break;
                case GameScreen.UpgradesAndEvolution:
                    _upgradeButton.SetupPin(data);
                    break;
                case GameScreen.Shop:
                    _shopButton.SetupPin(data);
                    break;
                case GameScreen.GeneralCards:
                    _cardsButton.SetupPin(data);
                    break;
            }
        }

        private void Unsubscribe()
        {
            _upgradesChecker.Notify -= OnUpgradesNotified;
            GlobalEvents.OnInsufficientFunds -= OnInsufficientFunds;
        }

        private void OnInsufficientFunds() => OnShopButtonClick(_shopButton);


        private void OnBattleButtonClick(ToggleButton button) => 
            _menuStateMachine.Enter<BattleState>();

        private void OnUpgradeButtonClick(ToggleButton button)
        {
            _menuStateMachine.Enter<UpgradesState>();
        }

        private void OnShopButtonClick(ToggleButton button) => 
            _menuStateMachine.Enter<ShopState>();

        private void OnCardsButtonClick(ToggleButton obj) => 
            _menuStateMachine.Enter<GeneralCardsState>();

        private void OnDungeonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        public void Hide()
        {
            HideCurtain();
            Unsubscribe();
            _upgradeButton.Cleanup();
            _battleButton.Cleanup();
            _dungeonButton.Cleanup();
            _cardsButton.Cleanup();
            _shopButton.Cleanup();
            
            _menuStateMachine.Cleanup();
            
            UpgradeTutorialStep.CancelStep();
            _tutorialManager.UnRegister(UpgradeTutorialStep);
            _tutorialManager.UnRegister(CardsTutorialStep);
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
