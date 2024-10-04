using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._CardsGeneral._Summoning.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreen : MonoBehaviour, IGameScreen
    {
        public GameScreen GameScreen => GameScreen.Cards;
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CardsContainer _container;

        [SerializeField] private Button _summoningButton;
        [SerializeField] private Slider _summoningProgressSlider;
        [SerializeField] private TMP_Text _summoningProgressLabel;
        [SerializeField] private TMP_Text _summoningLevelLabel;
        
        [SerializeField] private TransactionButton _x1CardBtn;
        [SerializeField] private TransactionButton _x10CardBtn;

        [SerializeField] private TutorialStep _cardsTutorialStep;
        
        private IWorldCameraService _cameraService;
        private IAudioService _audioService;
        private ICardsScreenPresenter _cardsScreenPresenter;
        private IHeader _header;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        private ICardsPresenter _cardsPresenter;
        private ITutorialManager _tutorialManager;

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            ICardsScreenPresenter cardsScreenPresenter,
            IUIFactory uiFactory, 
            IMyLogger logger,
            IHeader header,
            IUpgradesAvailabilityChecker upgradesChecker,
            ICardsPresenter cardsPresenter,
            ITutorialManager tutorialManager)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsScreenPresenter = cardsScreenPresenter;
            _header = header;
            _upgradesChecker = upgradesChecker;
            _cardsPresenter = cardsPresenter;
            _tutorialManager = tutorialManager;

            _container.Construct(cardsPresenter, uiFactory, audioService, logger);
            Init();
        }

        private void Init()
        {
            Unsubscribe();
            Subscribe();
            
            _tutorialManager.Register(_cardsTutorialStep);
            
            UpdateButtons(_cardsScreenPresenter.ButtonModels);
            UpdateSummoningView(_cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModel);
            
            _x1CardBtn.Init();
            _x10CardBtn.Init();
            _container.Init();
        }

        private void UpdateSummoningView(
            CardsSummoningModel model)
        {
            _summoningLevelLabel.text = model.CurrentLevel.ToString();
            _summoningProgressLabel.text = model.Progress;
            _summoningProgressSlider.value = model.ProgressValue;
        }

        public void Show()
        {
            _cardsScreenPresenter.OnCardsScreenOpened();
            
            UpdateScreenName();
            
            _upgradesChecker.MarkAsReviewed(GameScreen);
            _upgradesChecker.MarkAsReviewed(GameScreen.GeneralCards);
        }

        private void UpdateScreenName()
        {
            var screenName = GameScreen.ToString();
            var smallerFontSize = 50;
            var fullName = $"{screenName} \n<size={smallerFontSize}%>{_cardsScreenPresenter.CardsCountInfo}</size>";
            _header.ShowScreenName(fullName, Color.white);
        }

        private void Subscribe()
        {
            _x10CardBtn.ButtonStateChanged += OnButtonStateChanged;
            _summoningButton.onClick.AddListener(OnSummoningButtonClicked);
            _x1CardBtn.Click += OnX1CardBtnClicked;
            _x10CardBtn.Click += OnX10CardBtnClicked;
            _cardsScreenPresenter.ButtonModelsChanged += UpdateButtons;
            _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModelChanged += UpdateSummoningView;
            _cardsPresenter.CardModelUpdated += OnCardModelUpdated;
        }

        private void Unsubscribe()
        {
            _summoningButton.onClick.RemoveAllListeners();
            _x1CardBtn.Click -= OnX1CardBtnClicked;
            _x10CardBtn.Click -= OnX10CardBtnClicked;
            _cardsScreenPresenter.ButtonModelsChanged -= UpdateButtons;
            _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModelChanged -= UpdateSummoningView;
            _cardsPresenter.CardModelUpdated -= OnCardModelUpdated;
            _x10CardBtn.ButtonStateChanged -= OnButtonStateChanged;
        }

        private void OnButtonStateChanged(ButtonState state)
        {
            if (state == ButtonState.Active)
            {
                _cardsTutorialStep.ShowStep();
            }
            else if(state == ButtonState.Inactive)
            {
                _cardsTutorialStep.CancelStep();
            }
        }

        private void UpdateButtons(TransactionButtonModel[] models)
        {
            _x1CardBtn.UpdateButtonState(models[0]);
            _x10CardBtn.UpdateButtonState(models[1]);
        }

        private void OnX10CardBtnClicked()
        {
            _cardsScreenPresenter.TryToBuyX10Card();
            _cardsTutorialStep.CompleteStep();
            PlayButtonSound();
        }

        private void OnX1CardBtnClicked()
        {
            _cardsScreenPresenter.TryToBuyX1Card();
            PlayButtonSound();
        }

        private async void OnSummoningButtonClicked()
        {
            PlayButtonSound();
            ISummoningPopupProvider summoningPopupProvider 
                = new SummoningPopupProvider(_cameraService, _audioService, _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModel);
            var summoningPopup = await summoningPopupProvider.Load();
            await summoningPopup.Value.AwaitForExit();
            summoningPopup.Value.Cleanup();
            summoningPopup.Dispose();
            
        }

        private void OnCardModelUpdated(int _, CardModel __) => UpdateScreenName();

        public void Hide()
        {
            _cardsTutorialStep.CancelStep();
            _tutorialManager.UnRegister(_cardsTutorialStep);
            Unsubscribe();
            Cleanup();
        }

        private void Cleanup()
        {
            _x1CardBtn.Cleanup();
            _x10CardBtn.Cleanup();
            _container.Cleanup();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}