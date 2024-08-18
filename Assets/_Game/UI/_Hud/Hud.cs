using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Gameplay._DailyTasks.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._Hud._BattleSpeedView;
using _Game.UI._Hud._CoinCounterView;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._Hud._FoodBoostView;
using _Game.UI._Hud._PauseView;
using _Game.UI._Hud._SpeedBoostView.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
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
        [SerializeField] private DailyTaskView _dailyTaskView;

        public CoinCounterView CounterView => _counterView;

        public void Construct(
            IWorldCameraService cameraService,
            IAlertPopupProvider alertPopupProvider,
            IAudioService audioService,
            IFoodBoostService foodBoostService,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeed,
            ISpeedBoostService speedBoost,
            IBattleManager battleManager,
            IDailyTaskPresenter dailyTaskPresenter,
            ITutorialManager tutorialManager,
            IMyLogger logger)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;

            _foodBoostView.Construct(foodBoostService, audioService);
            _pauseView.Construct(audioService, featureUnlockSystem, alertPopupProvider,  battleManager, this);
            _battleSpeedView.Construct(battleSpeed, audioService);
            _speedBoostView.Construct(speedBoost, audioService);
            _counterView.Construct();
            _dailyTaskView.Construct(dailyTaskPresenter, audioService, tutorialManager, logger);
        }

        public void Init()
        {
            _dailyTaskView.Init();
            _speedBoostView.Init();
            _pauseView.Init();
            _foodBoostView.Init();
            Show();
        }

        private void OnDestroy()
        {
            _dailyTaskView.Cleanup();
            _speedBoostView.Cleanup();
            _pauseView.Cleanup();
            _foodBoostView.Cleanup();
        }

        public void Show()
        {
            _canvas.enabled = true;
            _speedBoostView.Show();
            _dailyTaskView.Show();
            _pauseView.Show();
            _foodBoostView.Show();
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _dailyTaskView.Hide();
            _pauseView.Hide();
            _foodBoostView.Hide();
        }

        public void ShowCoinCounter() => 
            _counterView.Show();

        public void HideCoinCounter()
        {
            _counterView.Clear();
            _counterView.Hide();
        }

        public void OnCoinsCoinsChanged(float amount) => 
            _counterView.UpdateCoins(amount);


        public void ShowFoodBoostBtn() => 
            _foodBoostView.Show();

        public void HideFoodBoostBtn() => 
            _foodBoostView.Hide();


        public void ShowPauseToggle() => 
            _pauseView.Show();

        public void HidePauseToggle() => 
            _pauseView.Hide();

        public void Quit() => 
            QuitBattle?.Invoke();
    }
}