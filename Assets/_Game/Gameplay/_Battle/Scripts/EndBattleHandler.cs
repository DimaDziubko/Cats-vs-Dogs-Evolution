using System.Collections.Generic;
using _Game._BattleModes.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core._GameSaver;
using _Game.Core.Loading;
using _Game.Core.Navigation.Battle;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._BattleStateHandler;
using _Game.UI._BattleUIController;
using _Game.UI.GameResult.Scripts;
using Assets._Game.Core.Loading;
using Assets._Game.Gameplay.GameResult.Scripts;

namespace _Game.Gameplay._Battle.Scripts
{
    public class EndBattleHandler : IEndBattleListener
    {
        private readonly BattleMode _battleMode;
        private readonly Battle _battle;
        private readonly BattleUIController _battleUIController;
        private readonly BattleStateHandler _battleStateHandler;
        private readonly GameResultHandler _gameResultHandler;
        private readonly IUserContainer _userContainer;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IGameSaver _gameSaver;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ISoundService _soundService;
        
        public EndBattleHandler(
            BattleMode battleMode,
            BattleUIController uiController,
            BattleStateHandler battleStateHandler,
            GameResultHandler gameResultHandler,
            IUserContainer userContainer,
            IBattleNavigator battleNavigator,
            ISoundService soundService)
        {
            _battleMode = battleMode;
            _battleUIController = uiController;
            _battleStateHandler = battleStateHandler;
            _gameResultHandler = gameResultHandler;
            _battleNavigator = battleNavigator;
            _soundService = soundService;
            _userContainer = userContainer;
        }
        
        async  void IEndBattleListener.OnEndBattle(GameResultType result, bool wasExit = false)
        {
            if (!wasExit) _userContainer.AnalyticsStateHandler.AddCompletedBattle();
            
            var isConfirmed = await _gameResultHandler.ShowGameResultAndWaitForDecision(result, wasExit);
            
            if (isConfirmed)
            {
                if(result == GameResultType.Victory) _battleNavigator.OpenNextBattle();
                GoToMainMenu();
            }
        }

        private void GoToMainMenu()
        {
            Queue <ILoadingOperation> loadingOperations = new Queue<ILoadingOperation>();
            loadingOperations.Enqueue(new ClearGameOperation(
                _battleMode,
                _soundService));
            _battleUIController.HideCoinCounter();
            _battleUIController.ShowRewardCoinsAfterLoading();
            _battleStateHandler.GoToMainMenu(loadingOperations);
        }

    }
}