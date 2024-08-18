using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Providers;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class ConfigurationState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IUserContainer _userContainer;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly IMyLogger _logger;
        private readonly ILocalConfigProvider _localConfigProvider;

        public ConfigurationState(
            IGameStateMachine stateMachine,
            IUserContainer userContainer,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IMyLogger logger)
        {
            _stateMachine = stateMachine;
            _userContainer = userContainer;
            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
        }

        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(new ConfigOperation(
                _userContainer,
                _remoteConfigProvider,
                _localConfigProvider,
                _logger));
            
            _stateMachine.Enter<DataLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}