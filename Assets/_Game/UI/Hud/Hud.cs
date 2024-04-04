using System;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Camera;
using _Game.UI.Common.Scripts;
using _Game.Utils.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Hud
{
    public class Hud : MonoBehaviour
    {
        public event Action Opened;
        
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _quitButton;
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;
        [SerializeField] private CoinCounterView _counterView;

        [SerializeField] private FoodBoostBtn _foodBoostBtn;

        public event Action QuitBattle;
        
        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private IAudioService _audioService;
        private IBonusRewardService _bonusRewardService;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IBonusRewardService bonusRewardService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _audioService = audioService;
            _bonusRewardService = bonusRewardService;

            Hide();
        }

        public void Show()
        {
            _canvas.enabled = true;
            
            _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);

            Unsubscribe();
            Subscribe();
            
            Opened?.Invoke();
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _quitButton.onClick.RemoveAllListeners();
            
            _foodBoostBtn.Cleanup();
            
            Unsubscribe();

            _counterView.Clear();
        }

        public void OnCoinsChanged(float amount)
        {
            _counterView.UpdateCoins(amount);
        }

        private void Subscribe()
        {
            _bonusRewardService.FoodBoostBtnModelChanged += _foodBoostBtn.UpdateBtnState;

            _pauseToggle.ValueChanged += OnPauseClicked;

            Opened += _bonusRewardService.OnHudOpened;
        }

        private void Unsubscribe()
        {
            _bonusRewardService.FoodBoostBtnModelChanged -= _foodBoostBtn.UpdateBtnState;

            _pauseToggle.ValueChanged -= OnPauseClicked;

            Opened -= _bonusRewardService.OnHudOpened;
        }

        private void OnFoodBoostBtnClicked()
        {
            _audioService.PlayButtonSound();
            _pauseManager.SetPaused(true);
            _bonusRewardService.OnFoodBoostBtnClicked();
        }

        private void OnPauseClicked(bool isPaused)
        {
            _audioService.PlayButtonSound();
            _pauseManager.SetPaused(isPaused);
            ShowAlertPopup();
        }

        private async void ShowAlertPopup()
        {
            var popup = await _alertPopupProvider.Load();
            var isConfirmed = await popup.Value.AwaitForDecision("End battle?");
            
            _pauseManager.SetPaused(false);
            _pauseToggle.UpdateToggleStateManually(false);
            
            if (isConfirmed)
            {
                QuitBattle?.Invoke();
            }
            
            popup.Dispose();
        }
    }
}