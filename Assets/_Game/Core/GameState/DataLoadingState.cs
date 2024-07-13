using System.Collections.Generic;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.AgeDataProvider;
using _Game.Core.DataProviders.Timeline;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class DataLoadingState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IBattleDataProvider _battleDataProvider;
        private readonly IMyLogger _logger;
        private readonly IGameStateMachine _stateMachine;
        private readonly ITimelineDataProvider _timelineDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;

        public DataLoadingState(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IBattleDataProvider battleDataProvider,
            ITimelineDataProvider timelineDataProvider,
            IMyLogger logger,
            IGameStateMachine stateMachine,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer)
        {
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _battleDataProvider = battleDataProvider;
            _timelineDataProvider = timelineDataProvider;
            _logger = logger;
            _stateMachine = stateMachine;
            _assetRegistry = assetRegistry;
            _userContainer = userContainer;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(new DataLoadingOperation(
                _generalDataPool,
                _ageDataProvider,
                _battleDataProvider,
                _timelineDataProvider,
                _assetRegistry,
                _userContainer,
                _logger));

            _stateMachine.Enter<InitializationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}