using System.Collections.Generic;
using Assets._Game.Core.Communication;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;
using Assets._Game.Core.Login;
using Assets._Game.Core.Services.Random;
using Assets._Game.Core.Services.UserContainer;

namespace _Game.Core.GameState
{
    public class LoginState : IState
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
        
        public void Enter()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            
            loadingOperations.Enqueue(new LoginOperation(
                _userContainer,
                _communicator,
                _random));
            
            _stateMachine.Enter<ConfigurationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}