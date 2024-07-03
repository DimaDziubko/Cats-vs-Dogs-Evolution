using System.Collections.Generic;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core.Loading;

namespace Assets._Game.Core.GameState
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