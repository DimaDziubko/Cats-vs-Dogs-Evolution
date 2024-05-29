using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._SpeedBoostBtn.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Popups; 
using UnityEngine;

namespace _Game.UI._Hud
{
    public class Hud : MonoBehaviour
    {
        public event Action QuitBattle;
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ToggleWithSpriteSwap _pauseToggle;
        [SerializeField] private CoinCounterView _counterView;
        [SerializeField] private FoodBoostBtn _foodBoostBtn;
        [SerializeField] private BattleSpeedBtn _battleSpeedBtn;
        
        public CoinCounterView CounterView => _counterView;

        private IPauseManager _pauseManager;
        private IAlertPopupProvider _alertPopupProvider;
        private IAudioService _audioService;
        private IFoodBoostService _foodBoostService;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IBattleSpeedService _battleSpeed;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IFoodBoostService foodBoostService,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeed)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _pauseManager = pauseManager;
            _alertPopupProvider = alertPopupProvider;
            _audioService = audioService;
            _foodBoostService = foodBoostService;
            _featureUnlockSystem = featureUnlockSystem;
            _battleSpeed = battleSpeed;

            Show();
        }

        public void Show()
        {
            _canvas.enabled = true;
            ShowBattleSpeedBtn();
            HideCoinCounter();
            HideFoodBoostBtn();
            HidePauseToggle();
        }

        public void ShowCoinCounter()
        {
            _counterView.Show();
        }

        public void HideCoinCounter()
        {
            _counterView.Clear();
            _counterView.Hide();
        }

        public void ShowFoodBoostBtn()
        {
            SubscribeFoodBoostBtn();
            _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);
            _foodBoostBtn.Show();
            OnFoodBoostBtnShown();
        }

        private void OnFoodBoostBtnShown()
        {
            _foodBoostService.OnFoodBoostShown();
        }

        private void SubscribeFoodBoostBtn()
        {
            _foodBoostService.FoodBoostBtnModelChanged += _foodBoostBtn.UpdateBtnState;
        }

        public void HideFoodBoostBtn()
        {
            _foodBoostBtn.Hide();
            _foodBoostBtn.Cleanup();
            UnsubscribeFoodBoostBtn();
        }

        private void UnsubscribeFoodBoostBtn()
        {
            _foodBoostService.FoodBoostBtnModelChanged -= _foodBoostBtn.UpdateBtnState;
        }

        public void ShowBattleSpeedBtn()
        {
            SubscribeBattleSpeedButton();
            _battleSpeedBtn.Initialize(OnBattleSpeedBtnClicked);
            OnBattleSpeedBtnShown();
        }

        private void OnBattleSpeedBtnShown() => 
            _battleSpeed.OnBattleSpeedBtnShown();

        private void SubscribeBattleSpeedButton()
        {
            _battleSpeed.SpeedBoostTimerActivityChanged += OnTimerChanged;
            _battleSpeed.BattleSpeedBtnModelChanged += _battleSpeedBtn.UpdateBtnState;
        }

        private void OnTimerChanged(GameTimer timer, bool isActive)
        {
            if (isActive)
            {
                timer.Tick += OnBattleSpeedTimerTick;
                return;
            }
            timer.Tick -= OnBattleSpeedTimerTick;
        }

        public void HideBattleSpeedBtn()
        {
            UnsubscribeBattleSpeedButton();
        }

        private void UnsubscribeBattleSpeedButton()
        {
            _battleSpeed.SpeedBoostTimerActivityChanged -= OnTimerChanged;
            _battleSpeed.BattleSpeedBtnModelChanged -= _battleSpeedBtn.UpdateBtnState;
        }
        
        public void ShowPauseToggle()
        {
            SubscribePauseToggle();
            var isPauseAvailable = _featureUnlockSystem.IsFeatureUnlocked(_pauseToggle);
            _pauseToggle.SetActive(isPauseAvailable);
        }

        private void SubscribePauseToggle()
        {
            _pauseToggle.ValueChanged += OnPauseClicked;
        }

        public void HidePauseToggle()
        {
            UnsubscribePauseToggle();
            _pauseToggle.SetActive(false);
        }

        private void UnsubscribePauseToggle() => 
            _pauseToggle.ValueChanged -= OnPauseClicked;

        public void Hide() => 
            _canvas.enabled = false;

        public void OnCoinsChanged(float amount) => 
            _counterView.UpdateCoins(amount);
        
        private void OnBattleSpeedTimerTick(float timeLeft) => 
            _battleSpeedBtn.UpdateTimer(timeLeft);
        

        private void OnBattleSpeedBtnClicked(BattleSpeedBtnState state)
        {
            _audioService.PlayButtonSound();
            _battleSpeed.OnBattleSpeedBtnClicked(state);
        }

        private void OnFoodBoostBtnClicked()
        {
            _audioService.PlayButtonSound();
            _foodBoostService.OnFoodBoostBtnClicked();
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