using System;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.AgeDataProvider;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.Loading
{
    public class AgeDataLoadingOperation : ILoadingOperation
    {
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeDataProvider _ageDataProvider;
        private readonly IAssetRegistry _assetRegistry;
        public string Description => "Loading age...";

        public AgeDataLoadingOperation(
            IGeneralDataPool generalDataPool,
            IAgeDataProvider ageDataProvider,
            IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
            _generalDataPool = generalDataPool;
            _ageDataProvider = ageDataProvider;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.2f);
            _generalDataPool.CleanupAgeData();
            onProgress.Invoke(0.4f);
            _assetRegistry.ClearContext(Constants.CacheContext.AGE);
            onProgress.Invoke(0.6f);
            _generalDataPool.AgeStaticData = await _ageDataProvider.Load();
            onProgress.Invoke(1.0f);
        }
    }
}