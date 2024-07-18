using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Battle.Scripts;
using _Game.UI._Hud;
using _Game.UI.UnitBuilderBtn.Scripts;
using Assets._Game.Common;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.LoadingScreen;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services._BattleSpeedService._Scripts;
using Assets._Game.Core.Services._FoodBoostService.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.Utils.Popups;

namespace _Game.UI._BattleUIController
{
    public class BattleUIController
    {
        private readonly Hud _hud;
        private readonly GameplayUI _gameplayUI;
        private readonly ICoinCounter _coinCounter;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IHeader _header;
        private readonly IRewardAnimator _rewardAnimator;
        private readonly IUserContainer _userContainer;

        private IBattleMediator _battleMediator;

        public BattleUIController(
            Hud hud,
            GameplayUI gameplayUI,
            ICoinCounter coinCounter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IFoodBoostService foodBoostService,
            IHeader header,
            IUserContainer userContainer,
            ILoadingScreenProvider loadingScreenProvider,
            IRewardAnimator rewardAnimator,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeedService)
        {
            _userContainer = userContainer;
            _hud = hud;
            _coinCounter = coinCounter;
            _loadingScreenProvider = loadingScreenProvider;
            _rewardAnimator = rewardAnimator;
            _gameplayUI = gameplayUI;

            _hud.Construct(
                cameraService,
                pauseManager,
                alertPopupProvider,
                audioService,
                foodBoostService,
                featureUnlockSystem,
                battleSpeedService);
            
            header.Construct(userContainer.State.Currencies, cameraService);
            _header = header;
        }
        
        public void OnStartBattle()
        {
            _hud.ShowCoinCounter();
            _hud.ShowPauseToggle();
            _hud.ShowFoodBoostBtn();
            
            Unsubscribe();
            Subscribe();
            _hud.OnCoinsChanged(_coinCounter.Coins);
        }
        
        public void OnStopBattle()
        {
            _hud.HidePauseToggle();
            _hud.HideFoodBoostBtn();
            Unsubscribe();
        }
        
        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }

        public void HideCoinCounter()
        {
            _coinCounter.Changed -= _hud.OnCoinsChanged;
            _hud.HideCoinCounter();
        }

        public void ShowRewardCoinsAfterLoading()
        {
            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
        }

        private void OnLoadingCompleted()
        {
            if (_coinCounter.Coins > 0)
            {
                PlayCoinAnimation();
                _userContainer.AddCoins(_coinCounter.Coins);
                _coinCounter.Cleanup();
            }
            
            _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
        }

        private void PlayCoinAnimation()
        {
            _rewardAnimator.PlayCoins(
                _header.CoinsWalletWorldPosition,
                _coinCounter.CoinsRation);
        }

        private void Subscribe()
        {
            _hud.QuitBattle += OnBattleQuit;
            _coinCounter.Changed += _hud.OnCoinsChanged;
        }

        private void Unsubscribe()
        {
            _hud.QuitBattle -= OnBattleQuit;
        }

        private void OnBattleQuit()
        {
            _battleMediator.StopBattle();
            _battleMediator.EndBattle(GameResultType.Defeat, true);
        }

        public void UpdateWave(int currentWave) => 
            _gameplayUI.WaveInfoPopup.ShowWave(currentWave);
    }
}