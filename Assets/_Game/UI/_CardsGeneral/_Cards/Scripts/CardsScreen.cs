using _Game.UI._CardsGeneral._Summoning.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreen : MonoBehaviour
    {
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
        
        public void Construct(
            IWorldCameraService cameraService, 
            IAudioService audioService,
            ICardsScreenPresenter cardsScreenPresenter)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _cameraService = cameraService;
            _audioService = audioService;
            _cardsScreenPresenter = cardsScreenPresenter;
            Init();
        }

        private void Init()
        {
            UpdateButtons(_cardsScreenPresenter.ButtonModels);
            UpdateSummoningView(_cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModel);
            _x1CardBtn.Init();
            _x10CardBtn.Init();
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
        }

        private void Subscribe()
        {
            _summoningButton.onClick.AddListener(OnSummoningButtonClicked);
            _x1CardBtn.Click += OnX1CardBtnClicked;
            _x10CardBtn.Click += OnX10CardBtnClicked;
            _cardsScreenPresenter.ButtonModelsChanged += UpdateButtons;
            _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModelChanged += UpdateSummoningView;
        }

        private void UpdateButtons(TransactionButtonModel[] models)
        {
            _x1CardBtn.UpdateButtonState(models[0]);
            _x10CardBtn.UpdateButtonState(models[1]);
        }

        private void OnX10CardBtnClicked()
        {
            _cardsScreenPresenter.TryToBuyX10Card();
        }

        private void OnX1CardBtnClicked()
        {
            _cardsScreenPresenter.TryToBuyX1Card();
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
        }

        public void Hide()
        {
            Unsubscribe();
            Cleanup();
        }

        public void Cleanup()
        {
            _x1CardBtn.Cleanup();
            _x10CardBtn.Cleanup();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}