using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Camera;
using _Game.Core.UserState;
using _Game.UI.Common.Scripts;
using _Game.Utils.Popups;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class Hud : MonoBehaviour
    {
        public event Action Opened;
        
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;
        [SerializeField] private CoinCounterView _counterView;

        [SerializeField] private FoodBoostBtn _foodBoostBtn;

        public CoinCounterView CounterView => _counterView;
        
        public event Action QuitBattle;
        
        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private IAudioService _audioService;
        private IBonusRewardService _bonusRewardService;
        private IFeatureUnlockSystem _featureUnlockSystem;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IBonusRewardService bonusRewardService,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _audioService = audioService;
            _bonusRewardService = bonusRewardService;
            _featureUnlockSystem = featureUnlockSystem;

            Hide();
        }

        public void Show()
        {
            _canvas.enabled = true;

            var isFoodBoostAvailable = _featureUnlockSystem.IsFeatureUnlocked(_foodBoostBtn);

            _foodBoostBtn.SetActive(isFoodBoostAvailable);
            
            if (isFoodBoostAvailable)
            {
                _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);
                _bonusRewardService.FoodBoostBtnModelChanged += _foodBoostBtn.UpdateBtnState;
            }

            var isPauseAvailable = _featureUnlockSystem.IsFeatureUnlocked(_pauseToggle);
            _pauseToggle.SetActive(isPauseAvailable);
            
            Unsubscribe();
            Subscribe();
            
            Opened?.Invoke();
        }

        public void Hide()
        {
            _canvas.enabled = false;

            _foodBoostBtn.Cleanup();
            
            Unsubscribe();

            _counterView.Clear();
        }

        public void OnCoinsChanged(float amount) => 
            _counterView.UpdateCoins(amount);

        private void Subscribe()
        {
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