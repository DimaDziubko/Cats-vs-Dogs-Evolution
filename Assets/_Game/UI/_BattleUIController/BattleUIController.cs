using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.LoadingScreen;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._DailyTasks.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._Currencies;
using _Game.UI._GameplayUI.Scripts;
using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Header.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.Utils.Popups;
using Zenject;

namespace _Game.UI._BattleUIController
{
    public class BattleUIController :
        IInitializable,
        IStartBattleListener,
        IStopBattleListener,
        IUIListener
    {
        private readonly Hud _hud;

        private readonly GameplayUI _gameplayUI;

        private readonly ICoinCounter _coinCounter;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IUserContainer _userContainer;
        private readonly IBattleManager _battleManager;

        public BattleUIController(
            Hud hud,
            GameplayUI gameplayUI,
            ICoinCounter coinCounter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IAlertPopupProvider alertPopupProvider,
            IFoodBoostService foodBoostService,
            IHeader header,
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeedService,
            ISpeedBoostService speedBoost,
            IBattleManager battleManager,
            IDailyTaskPresenter dailyTaskPresenter,
            ITutorialManager tutorialManager, 
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _hud = hud;
            _coinCounter = coinCounter;
            _loadingScreenProvider = loadingScreenProvider; ;
            _gameplayUI = gameplayUI;
            _battleManager = battleManager;

            
            _hud.Construct(
                cameraService,
                alertPopupProvider,
                audioService,
                foodBoostService,
                featureUnlockSystem,
                battleSpeedService,
                speedBoost,
                battleManager,
                dailyTaskPresenter,
                tutorialManager,
                logger,
                header);
            
            header.Construct(userContainer.State.Currencies, cameraService);
        }

        void IInitializable.Initialize()
        {
            _hud.Init();
        }

        void IStartBattleListener.OnStartBattle()
        {
            _hud.ShowCoinCounter();
            _hud.ShowPauseToggle();
            _hud.ShowFoodBoostBtn();
            
            Unsubscribe();
            Subscribe();
            _hud.OnCoinsCoinsChanged(_coinCounter.Coins);
        }

        void IStopBattleListener.OnStopBattle()
        {
            _gameplayUI.WaveInfoPopup.HideWave();
            _hud.HidePauseToggle();
            _hud.HideFoodBoostBtn();
            Unsubscribe();
        }

        public void HideCoinCounter()
        {
            _coinCounter.CoinsChanged -= _hud.OnCoinsCoinsChanged;
            _hud.HideCoinCounter();
        }

        public void ShowRewardCoinsAfterLoading() => 
            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;

        private void OnLoadingCompleted()
        {
            if (_coinCounter.Coins > 0)
            {
                _userContainer.CurrenciesHandler.AddCoins(_coinCounter.Coins, CurrenciesSource.Battle);
                _coinCounter.Cleanup();
            }
            
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
        }

        private void Subscribe()
        {
            _hud.QuitBattle += OnBattleQuit;
            _coinCounter.CoinsChanged += _hud.OnCoinsCoinsChanged;
        }

        private void Unsubscribe() => 
            _hud.QuitBattle -= OnBattleQuit;

        private void OnBattleQuit()
        {
            _gameplayUI.WaveInfoPopup.HideWave();
            _battleManager.StopBattle();
            _battleManager.EndBattle(GameResultType.Defeat, true);
        }

        public void UpdateWave(int currentWave, int wavesCount) => 
            _gameplayUI.WaveInfoPopup.ShowWave(currentWave, wavesCount);

        void IUIListener.OnScreenOpened(GameScreen gameScreen)
        {
            switch (gameScreen)
            {
                case GameScreen.Battle:
                    _hud.Show();
                    break;
                default:
                    _hud.Hide();
                    break;
            }
        }

        void IUIListener.OnScreenClosed(GameScreen gameScreen)
        {
            
        }
    }
    
}