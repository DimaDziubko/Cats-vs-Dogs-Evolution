using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core.Navigation.Battle;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Gameplay.GameResult.Scripts
{
    public class GameResultHandler
    {
        private readonly IGameResultWindowProvider _gameResultWindowProvider;
        private readonly ICoinCounter _coinCounter;
        private readonly IFeatureUnlockSystem _featureUnlock;
        private readonly IBattleNavigator _battleNavigator;

        public GameResultHandler(
            IGameResultWindowProvider gameResultWindowProvider,
            ICoinCounter coinCounter,
            IFeatureUnlockSystem featureUnlock,
            IBattleNavigator battleNavigator)
        {
            _gameResultWindowProvider = gameResultWindowProvider;
            _coinCounter = coinCounter;
            _featureUnlock = featureUnlock;
            _battleNavigator = battleNavigator;
        }
        
        public async UniTask<bool> ShowGameResultAndWaitForDecision(GameResultType result, bool wasExit = false)
        {
            if(_coinCounter.Coins == 0 && !wasExit && _battleNavigator.CurrentBattle == 0) _coinCounter.AddCoins(Constants.Money.MIN_COINS_PER_BATTLE);
            
            if (_coinCounter.Coins > 0 && _featureUnlock.IsFeatureUnlocked(Feature.X2))
            {
                var popup = await _gameResultWindowProvider.Load();
                var isExitConfirmed = await popup.Value.ShowAndAwaitForExit(_coinCounter, result);
                popup.Dispose();
                return isExitConfirmed;
            }
            
            return true; 
        }
    }
}