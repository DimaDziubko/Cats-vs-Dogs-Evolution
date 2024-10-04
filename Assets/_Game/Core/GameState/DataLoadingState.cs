using System.Collections.Generic;
using _Game.Core._DataLoaders.AgeDataProvider;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.DataProviders.BattleDataProvider;
using _Game.Core.DataProviders.ShopDataProvider;
using _Game.Core.DataProviders.Timeline;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.GameState;
using Assets._Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class DataLoadingState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataLoader _ageDataLoader;
        private readonly IBattleDataLoader _battleDataLoader;
        private readonly IMyLogger _logger;
        private readonly IGameStateMachine _stateMachine;
        private readonly ITimelineDataLoader _timelineDataLoader;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private readonly IShopDataLoader _shopDataLoader;

        public DataLoadingState(
            IGeneralDataPool generalDataPool,
            IAgeDataLoader ageDataLoader,
            IBattleDataLoader battleDataLoader,
            ITimelineDataLoader timelineDataLoader,
            IMyLogger logger,
            IGameStateMachine stateMachine,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer, 
            IShopDataLoader shopDataLoader)
        {
            _generalDataPool = generalDataPool;
            _ageDataLoader = ageDataLoader;
            _battleDataLoader = battleDataLoader;
            _timelineDataLoader = timelineDataLoader;
            _logger = logger;
            _stateMachine = stateMachine;
            _assetRegistry = assetRegistry;
            _userContainer = userContainer;
            _shopDataLoader = shopDataLoader;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(new DataLoadingOperation(
                _generalDataPool,
                _ageDataLoader,
                _battleDataLoader,
                _timelineDataLoader,
                _shopDataLoader,
                _assetRegistry,
                _userContainer,
                _logger
                ));

            _stateMachine.Enter<InitializationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}