using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreen : MonoBehaviour, IGameScreen
    {
        [SerializeField] private Canvas _canvas;
        public GameScreen GameScreen => GameScreen.GeneralCards;

        [SerializeField] private ToggleButton _cardsBtn;
        [SerializeField] private ToggleButton _skillsBtn;
        [SerializeField] private ToggleButton _runesBtn;
        [SerializeField] private ToggleButton _heroesBtn;

        [SerializeField] private QuickBoostInfoPanel _quickBoostInfoPanel;

        private LocalStateMachine _localStateMachine;
        private ToggleButton _activeBtn;

        private IFeatureUnlockSystem _featureUnlockSystem;
        private IAudioService _audioService;
        private IUpgradesAvailabilityChecker _upgradesChecker;

        private bool IsCardsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_cardsBtn);


        public void Construct(
            IWorldCameraService cameraService,
            ICardsScreenProvider provider, 
            IUINotifier uiNotifier,
            IFeatureUnlockSystem featureUnlockSystem,
            IAudioService audioService,
            IBoostDataPresenter boostDataPresenter,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _featureUnlockSystem = featureUnlockSystem;
            _audioService = audioService;
            _upgradesChecker = upgradesChecker;
            
            _localStateMachine = new LocalStateMachine();
            CardsState cardsState = new  CardsState(this, provider, _cardsBtn, uiNotifier);

            _localStateMachine.AddState(cardsState);
            
            _canvas.enabled = false;
            
            _quickBoostInfoPanel.Construct(
                boostDataPresenter,
                audioService,
                BoostSource.TotalBoosts);
        }

        public void Show()
        {
            Unsubscribe();
            Subscribe();
            _cardsBtn.Initialize(IsCardsUnlocked, OnCardsBtnClicked, PlayButtonSound,
                _upgradesChecker.GetNotificationData(GameScreen.Cards));
            
            OnCardsBtnClicked(_cardsBtn);
            _quickBoostInfoPanel.Init();
            _canvas.enabled = true;
            
            _upgradesChecker.MarkAsReviewed(GameScreen);
        }

        private void Unsubscribe()
        {
            _upgradesChecker.Notify -= OnUpgradesNotified;
        }

        private void Subscribe()
        {
            _upgradesChecker.Notify += OnUpgradesNotified;
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
                    break;
                case GameScreen.Shop:
                    break;
                case GameScreen.GeneralCards:
                    break;
                case GameScreen.Cards:
                    _cardsBtn.SetupPin(data);
                    break;
            }
        }

        private void OnCardsBtnClicked(ToggleButton button)
        {
            _quickBoostInfoPanel.Source = BoostSource.Cards;
            _localStateMachine.Enter<CardsState>();
        }

        public void Hide()
        {
            Unsubscribe();
            _canvas.enabled = false;
            _cardsBtn.Cleanup();
            _localStateMachine.Cleanup();
            _quickBoostInfoPanel.Cleanup();
        }

        public void SetActiveButton(ToggleButton button)
        {
            _activeBtn = button;
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}