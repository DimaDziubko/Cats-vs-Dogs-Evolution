using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Utils;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.UI.GameResult.Scripts
{
    public class GameResultHandler
    {
        private readonly IGameResultPopupProvider _gameResultPopupProvider;
        private readonly ICoinCounter _coinCounter;
        private readonly IFeatureUnlockSystem _featureUnlock;
        private readonly IBattleNavigator _battleNavigator;

        public GameResultHandler(
            IGameResultPopupProvider gameResultPopupProvider,
            ICoinCounter coinCounter,
            IFeatureUnlockSystem featureUnlock,
            IBattleNavigator battleNavigator)
        {
            _gameResultPopupProvider = gameResultPopupProvider;
            _coinCounter = coinCounter;
            _featureUnlock = featureUnlock;
            _battleNavigator = battleNavigator;
        }
        
        public async UniTask<bool> ShowGameResultAndWaitForDecision(GameResultType result, bool wasExit = false)
        {
            if(_coinCounter.Coins == 0 && !wasExit && _battleNavigator.CurrentBattle == 0) 
                _coinCounter.AddCoins(Constants.Money.MIN_COINS_PER_BATTLE);
            
            if (_coinCounter.Coins > 0 && _featureUnlock.IsFeatureUnlocked(Feature.X2))
            {
                var popup = await _gameResultPopupProvider.Load();
                var isExitConfirmed = await popup.Value.ShowAndAwaitForExit(_coinCounter, result);
                popup.Dispose();
                return isExitConfirmed;
            }
            
            return true; 
        }
    }
}