using _Game.Common;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.Gameplay.GameResult.Scripts;
using _Game.UI._Hud;
using _Game.UI.Common.Header.Scripts;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI._BattleUIController
{
    public class BattleUIController
    {
        private readonly Hud _hud;
        private readonly ICoinCounter _coinCounter;
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly IHeader _header;
        private readonly IRewardAnimator _rewardAnimator;
        private readonly IUserContainer _persistentData;

        private IBattleMediator _battleMediator;

        public BattleUIController(
            Hud hud,
            ICoinCounter coinCounter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IFoodBoostService foodBoostService,
            IHeader header,
            IUserContainer persistentData,
            ILoadingScreenProvider loadingScreenProvider,
            IRewardAnimator rewardAnimator,
            IFeatureUnlockSystem featureUnlockSystem,
            IBattleSpeedService battleSpeedService)
        {
            _persistentData = persistentData;
            _hud = hud;
            _coinCounter = coinCounter;
            _loadingScreenProvider = loadingScreenProvider;
            _rewardAnimator = rewardAnimator;

            _hud.Construct(
                cameraService,
                pauseManager,
                alertPopupProvider,
                audioService,
                foodBoostService,
                featureUnlockSystem,
                battleSpeedService);
            
            header.Construct(persistentData.State.Currencies, cameraService);
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
                _persistentData.AddCoins(_coinCounter.Coins);
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
            _coinCounter.Changed -= _hud.OnCoinsChanged;
        }

        private void OnBattleQuit()
        {
            _battleMediator.StopBattle();
            _battleMediator.EndBattle(GameResultType.Defeat, true);
        }
    }
}