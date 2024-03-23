using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Providers;
using _Game.Core.Loading;
using _Game.Core.Services.PersistentData;

namespace _Game.Core.GameState
{
    public class ConfigurationState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IPersistentDataService _persistentData;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly IMyLogger _logger;

        public ConfigurationState(IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IMyLogger logger)
        {
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
        }

        public void Enter()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            
            loadingOperations.Enqueue(new ConfigOperation(
                _persistentData,
                _remoteConfigProvider,
                _localConfigProvider,
                _logger));
            
            _stateMachine.Enter<LoginState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}