using System;
using _Game.Core.AssetManagement;
using _Game.Core.Data;
using _Game.Core.DataProviders.AgeDataProvider;
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
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IBattleDataProvider _battleDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public ChangingRaceOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IBattleDataProvider battleDataProvider,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer)
        {
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _battleDataProvider = battleDataProvider;
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
            var ageTask = _ageDataProvider.Load(TimelineState.TimelineId);
            var battleTask = _battleDataProvider.Load(TimelineState.TimelineId);
            var result = await UniTask.WhenAll(ageTask, battleTask);
            onProgress.Invoke(0.9f);
            _generalDataPool.AgeStaticData = result.Item1;
            _generalDataPool.BattleStaticData = result.Item2; 
            onProgress.Invoke(1.0f);
        }
    }
}