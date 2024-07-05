using System.Collections.Generic;
using Assets._Game.Core.Communication;
using Assets._Game.Core.Loading;
using Assets._Game.Core.Login;
using Assets._Game.Core.Services.Random;
using Assets._Game.Core.Services.UserContainer;

namespace Assets._Game.Core.GameState
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