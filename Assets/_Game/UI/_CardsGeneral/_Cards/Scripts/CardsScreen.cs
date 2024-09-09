﻿using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.UI._CardsGeneral._Summoning.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
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
        
        private IWorldCameraService _cameraService;
        private IAudioService _audioService;
        private ICardsScreenPresenter _cardsScreenPresenter;
        private IHeader _header;
        private IUpgradesAvailabilityChecker _upgradesChecker;

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            ICardsScreenPresenter cardsScreenPresenter,
            IUIFactory uiFactory, 
            IMyLogger logger,
            IHeader header,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsScreenPresenter = cardsScreenPresenter;
            _header = header;
            _upgradesChecker = upgradesChecker;

            _container.Construct(cardsScreenPresenter.CardsPresenter, uiFactory, audioService, logger);
            Init();
        }

        private void Init()
        {
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
            Unsubscribe();
            Subscribe();
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
            _summoningButton.onClick.AddListener(OnSummoningButtonClicked);
            _x1CardBtn.Click += OnX1CardBtnClicked;
            _x10CardBtn.Click += OnX10CardBtnClicked;
            _cardsScreenPresenter.ButtonModelsChanged += UpdateButtons;
            _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModelChanged += UpdateSummoningView;
            _cardsScreenPresenter.CardsPresenter.CardModelUpdated += OnCardModelUpdated;
        }

        private void UpdateButtons(TransactionButtonModel[] models)
        {
            _x1CardBtn.UpdateButtonState(models[0]);
            _x10CardBtn.UpdateButtonState(models[1]);
        }

        private void OnX10CardBtnClicked()
        {
            _cardsScreenPresenter.TryToBuyX10Card();
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

        private void Unsubscribe()
        {
            _summoningButton.onClick.RemoveAllListeners();
            _x1CardBtn.Click -= OnX1CardBtnClicked;
            _x10CardBtn.Click -= OnX10CardBtnClicked;
            _cardsScreenPresenter.ButtonModelsChanged -= UpdateButtons;
            _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModelChanged -= UpdateSummoningView;
            _cardsScreenPresenter.CardsPresenter.CardModelUpdated -= OnCardModelUpdated;
        }

        private void OnCardModelUpdated(int _, CardModel __) => UpdateScreenName();

        public void Hide()
        {
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