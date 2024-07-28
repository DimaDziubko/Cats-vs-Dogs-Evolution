using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.UI._Hud._BattleSpeedView;
using _Game.UI._Hud._CoinCounterView;
using _Game.UI._Hud._FoodBoostView;
using _Game.UI._Hud._PauseView;
using _Game.UI._Hud._SpeedBoostView.Scripts;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services._FoodBoostService.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Utils.Popups;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class Hud : MonoBehaviour
    {
        public event Action QuitBattle;
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CoinCounterView _counterView;
        [SerializeField] private FoodBoostView _foodBoostView;
        [SerializeField] private PauseView _pauseView;
        [SerializeField] private BattleSpeedView _battleSpeedView;
        [SerializeField] private SpeedBoostView _speedBoostView;
        
        public CoinCounterView CounterView => _counterView;

        public void Construct(
            IWorldCameraService cameraService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IFoodBoostService foodBoostService,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeed,
            ISpeedBoostService speedBoost)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;

            _foodBoostView.Construct(foodBoostService, audioService);
            _pauseView.Construct(audioService, featureUnlockSystem, pauseManager, alertPopupProvider, this);
            _battleSpeedView.Construct(battleSpeed, audioService);
            _speedBoostView.Construct(speedBoost, audioService);
            _counterView.Construct();
            
            Show();
        }

        private void Show()
        {
            _canvas.enabled = true;
        }

        public void Hide() => 
            _canvas.enabled = false;

        public void ShowCoinCounter() => 
            _counterView.Show();

        public void HideCoinCounter()
        {
            _counterView.Clear();
            _counterView.Hide();
        }

        public void OnCoinsChanged(float amount) => 
            _counterView.UpdateCoins(amount);
        

        public void ShowFoodBoostBtn() => 
            _foodBoostView.ShowFoodBoostBtn();

        public void HideFoodBoostBtn() => 
            _foodBoostView.HideFoodBoostBtn();


        public void ShowPauseToggle() => 
            _pauseView.ShowPauseToggle();

        public void HidePauseToggle() => 
            _pauseView.HidePauseToggle();

        public void Quit() => 
            QuitBattle?.Invoke();
    }
}