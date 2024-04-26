using _Game.Common;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Loading;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.BonusReward.Scripts;
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

        private IBattleMediator _battleMediator;

        public BattleUIController(
            Hud hud,
            ICoinCounter coinCounter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IPauseManager pauseManager,
            IAlertPopupProvider alertPopupProvider,
            IBonusRewardService bonusRewardService,
            IHeader header,
            IPersistentDataService persistentData,
            ILoadingScreenProvider loadingScreenProvider,
            IRewardAnimator rewardAnimator,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _hud = hud;
            _coinCounter = coinCounter;
            _loadingScreenProvider = loadingScreenProvider;
            _rewardAnimator = rewardAnimator;

            _hud.Construct(
                cameraService,
                pauseManager,
                alertPopupProvider,
                audioService,
                bonusRewardService,
                featureUnlockSystem);
            
            header.Construct(persistentData.State.Currencies, cameraService);
            _header = header;
        }
        
        public void ShowHud()
        {
            _hud.Show();
            Unsubscribe();
            Subscribe();
            _hud.OnCoinsChanged(_coinCounter.Coins);
        }
        
        public void HideHud()
        {
            _hud.Hide();
            Unsubscribe();
        }

        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }

        public void HideAndHandleLoadingOperation(ILoadingOperation clearingOperation)
        {
            _loadingScreenProvider.LoadAndDestroy(clearingOperation).Forget();

            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
        }

        private void OnLoadingCompleted()
        {
            if (_coinCounter.Coins > 0)
            {
                _rewardAnimator.PlayCoins(
                    _header.CoinsWalletWorldPosition,
                    _coinCounter.Coins,
                    _coinCounter.CoinsRation);
                
                _coinCounter.Cleanup();
            }
        }

        private void Unsubscribe()
        {
            _hud.QuitBattle -= OnBattleQuit;
            _coinCounter.Changed -= _hud.OnCoinsChanged;
        }

        private void Subscribe()
        {
            _hud.QuitBattle += OnBattleQuit;
            _coinCounter.Changed += _hud.OnCoinsChanged;
        }
        
        private void OnBattleQuit()
        {
            _battleMediator.StopBattle();
            _battleMediator.EndBattle(GameResultType.Defeat, true);
        }
        
    }
}