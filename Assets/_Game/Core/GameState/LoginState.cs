using System.Collections.Generic;
using _Game.Core.Communication;
using _Game.Core.Loading;
using _Game.Core.Login;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;

namespace _Game.Core.GameState
{
    public class LoginState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IUserStateCommunicator _communicator;
        private readonly IRandomService _random;
        private readonly IGameStateMachine _stateMachine;

        public LoginState(
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IUserStateCommunicator communicator,
            IRandomService random)
        {
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _communicator = communicator;
            _random = random;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(new LoginOperation(
                _persistentData,
                _communicator,
                _random));
            
            _stateMachine.Enter<InitializationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}