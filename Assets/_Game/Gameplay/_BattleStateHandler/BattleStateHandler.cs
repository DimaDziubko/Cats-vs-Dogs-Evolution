using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;

namespace _Game.Gameplay._BattleStateHandler
{
    public class BattleStateHandler : IStartBattleListener
    {
        private readonly IGameStateMachine _stateMachine;
        
        public BattleStateHandler(IGameStateMachine stateMachine) => 
            _stateMachine = stateMachine;

        void IStartBattleListener.OnStartBattle() => 
            _stateMachine.Enter<GameLoopState>();

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