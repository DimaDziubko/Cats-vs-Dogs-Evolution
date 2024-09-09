using System;
using _Game.Core._DataLoaders.AgeDataProvider;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.Loading;
using Assets._Game.Core.UserState;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class ChangingRaceOperation : ILoadingOperation
    {
        public string Description => "Changing race...";

        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataLoader _ageDataLoader;
        private readonly IBattleDataLoader _battleDataLoader;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public ChangingRaceOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataLoader ageDataLoader,
            IBattleDataLoader battleDataLoader,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer)
        {
            _generalDataPool = generalDataPool;
            _ageDataLoader = ageDataLoader;
            _battleDataLoader = battleDataLoader;
            _assetRegistry = assetRegistry;
            _userContainer = userContainer;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.2f);
            _generalDataPool.CleanupAgeData();
            onProgress.Invoke(0.3f);
            _generalDataPool.CleanupBattleData();
            onProgress.Invoke(0.5f);
            _assetRegistry.ClearContext( TimelineState.TimelineId, Constants.CacheContext.AGE);
            onProgress.Invoke(0.7f);
            _assetRegistry.ClearContext(TimelineState.TimelineId, Constants.CacheContext.BATTLE);
            onProgress.Invoke(0.8f);
            var ageTask = _ageDataLoader.Load(TimelineState.TimelineId);
            var battleTask = _battleDataLoader.Load(TimelineState.TimelineId);
            var result = await UniTask.WhenAll(ageTask, battleTask);
            onProgress.Invoke(0.9f);
            _generalDataPool.AgeStaticData = result.Item1;
            _generalDataPool.BattleStaticData = result.Item2; 
            onProgress.Invoke(1.0f);
        }
    }
}