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
        private readonly IUserContainer _userContainer;
        private readonly IUserStateCommunicator _communicator;
        private readonly IRandomService _random;
        private readonly IGameStateMachine _stateMachine;

        public LoginState(
            IGameStateMachine stateMachine,
            IUserContainer userContainer,
            IUserStateCommunicator communicator,
            IRandomService random)
        {
            _stateMachine = stateMachine;
            _userContainer = userContainer;
            _communicator = communicator;
            _random = random;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(new LoginOperation(
                _userContainer,
                _communicator,
                _random));
            
            _stateMachine.Enter<DataLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}