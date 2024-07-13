using System;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.AgeDataProvider;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.Data;
using Assets._Game.Core.Loading;
using Assets._Game.Core.UserState;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class AgeDataLoadingOperation : ILoadingOperation
    {
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        public string Description => "Loading age...";

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public AgeDataLoadingOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IAssetRegistry assetRegistry,
            IUserContainer userContainer)
        {
            _assetRegistry = assetRegistry;
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
            _userContainer = userContainer;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.2f);
            _generalDataPool.CleanupAgeData();
            onProgress.Invoke(0.4f);
            _assetRegistry.ClearContext(TimelineState.TimelineId, Constants.CacheContext.AGE);
            onProgress.Invoke(0.6f);
            _generalDataPool.AgeStaticData = await _ageDataProvider.Load(TimelineState.TimelineId);
            onProgress.Invoke(1.0f);
        }
    }
}