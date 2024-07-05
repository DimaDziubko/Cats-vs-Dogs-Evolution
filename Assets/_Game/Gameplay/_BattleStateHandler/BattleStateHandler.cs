using System.Collections.Generic;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;
using Assets._Game.Core.LoadingScreen;
using Assets._Game.Gameplay.Battle.Scripts;

namespace Assets._Game.Gameplay._BattleStateHandler
{
    public class BattleStateHandler
    {
        private readonly IGameStateMachine _stateMachine;
        
        private IBattleMediator _battleMediator;

        public BattleStateHandler(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void SetMediator(IBattleMediator battleMediator)
        {
            _battleMediator = battleMediator;
        }

        public void HandleStart()
        {
            _stateMachine.Enter<GameLoopState>();
        }

        public void GoToMainMenu(Queue<ILoadingOperation> operations)
        {
            LoadingData data = new LoadingData()
            {
                Type = LoadingScreenType.DarkFade,
                Operations = operations,
            };
            _stateMachine.Enter<MenuState, LoadingData>(data);
        }
    }
}