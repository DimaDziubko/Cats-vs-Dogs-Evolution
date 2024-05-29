using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay.GameResult.Scripts
{
    public class GameResultHandler
    {
        private readonly IGameResultWindowProvider _gameResultWindowProvider;
        private readonly ICoinCounter _coinCounter;
        private readonly IEconomyUpgradesService _economy;
        private readonly IFeatureUnlockSystem _featureUnlock;

        public GameResultHandler(
            IGameResultWindowProvider gameResultWindowProvider,
            ICoinCounter coinCounter,
            IEconomyUpgradesService  economy,
            IFeatureUnlockSystem featureUnlock)
        {
            _gameResultWindowProvider = gameResultWindowProvider;
            _coinCounter = coinCounter;
            _economy = economy;
            _featureUnlock = featureUnlock;
        }
        
        public async UniTask<bool> ShowGameResultAndWaitForDecision(GameResultType result, bool wasExit = false)
        {
            if(_coinCounter.Coins == 0 && !wasExit) _coinCounter.AddCoins(_economy.MinimalCoinsForBattle);
            
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