using System;
using _Game.Core._DataPresenters.Evolution;
using _Game.Core.Services.Audio;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Header.Scripts;
using _Game.UI.TimelineInfoScreen.Scripts;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionTab : MonoBehaviour
    {
        public event Action EvolutionTabOpened;
    
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Image _currentAgeImage;
        [SerializeField] private TMP_Text _currentAgeName;
        
        [SerializeField] private Image _nextAgeImage;
        [SerializeField] private TMP_Text _nextAgeName;
    
        [SerializeField] private TransactionButton _evolveButton;

        [SerializeField] private TMP_Text _timelineLabel;
        [SerializeField] private TMP_Text _difficultyLabel;
    
        private IDisposable _disposable;
        
        private IEvolutionPresenter _evolutionPresenter;
        private IHeader _header;
        private IAudioService _audioService;
        private ITimelineInfoScreenProvider _timelineInfoProvider;
        private IMiniShopProvider _miniShopProvider;

        public void Construct(
            IEvolutionPresenter evolutionPresenter,
            IAudioService audioService,
            ITimelineInfoScreenProvider timelineInfoProvider,
            IMiniShopProvider miniShopProvider)
        {
            _audioService = audioService;
            _evolutionPresenter = evolutionPresenter;
            _timelineInfoProvider = timelineInfoProvider;
            _miniShopProvider = miniShopProvider;
        }
    
        public void Show()
        {
            _evolveButton.Init();
            
            Unsubscribe();

            Subscribe();

            EvolutionTabOpened?.Invoke();
            
            _canvas.enabled = true;
        }

        private void Subscribe()
        {
            _evolveButton.Click += OnEvolveButtonClick;
            _evolveButton.InactiveClick += OnInactiveButtonClick;
            EvolutionTabOpened += _evolutionPresenter.OnEvolutionTabOpened;
            _evolutionPresenter.EvolutionModelUpdated += OnEvolutionViewModelUpdated;
        }

        private void Unsubscribe()
        {
            _evolveButton.Click -= OnEvolveButtonClick;
            _evolveButton.InactiveClick -= OnInactiveButtonClick;
            EvolutionTabOpened -= _evolutionPresenter.OnEvolutionTabOpened;
            _evolutionPresenter.EvolutionModelUpdated -= OnEvolutionViewModelUpdated;
        }

        private async void OnInactiveButtonClick()
        {
            if(!_miniShopProvider.IsUnlocked) return;
            var popup = await _miniShopProvider.Load();
            bool isExit = await popup.Value.ShowAndAwaitForDecision(_evolutionPresenter.GetEvolutionPrice());
            if(isExit)
            {
                _miniShopProvider.Unload();
            }
        }

        public void Hide()
        {
            Unsubscribe();
            _evolveButton.Cleanup();
            _disposable?.Dispose();
            _canvas.enabled = false;
        }

        private void OnEvolutionViewModelUpdated(EvolutionTabModel tabModel)
        {
            UpdateButtonData(tabModel.EvolutionBtnData);

            _timelineLabel.text = tabModel.TimelineInfo;
            _currentAgeImage.sprite = tabModel.CurrentAgeIcon;
            _nextAgeImage.sprite = tabModel.NextAgeIcon;
            _currentAgeName.text = tabModel.CurrentAgeName;
            _nextAgeName.text = tabModel.NextAgeName;
            _difficultyLabel.text = tabModel.Difficulty;
            _difficultyLabel.enabled = tabModel.ShowDifficulty;
        }

        private void UpdateButtonData(EvolutionBtnData data) => 
            _evolveButton.UpdateButtonState(data.ButtonState, data.Price.FormatMoney(),  "Evolve", true,data.Price > 0);

        private async void OnEvolveButtonClick()
        {
            PlayButtonSound();
            
            var screen = await _timelineInfoProvider.Load();
            var isExited = await screen.Value.ShowScreenWithTransitionAnimation();
            
            if(isExited) screen.Dispose();
            _disposable = screen;
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
    }
}