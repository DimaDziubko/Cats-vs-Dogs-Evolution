using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class InitializationState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IGameInitializer _gameInitializer;

        public InitializationState(
            IGameStateMachine stateMachine,
            IGameInitializer gameInitializer)
        {
            _stateMachine = stateMachine;
            _gameInitializer = gameInitializer;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new InitializationOperation(_gameInitializer));
            
            _stateMachine.Enter<GameLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}