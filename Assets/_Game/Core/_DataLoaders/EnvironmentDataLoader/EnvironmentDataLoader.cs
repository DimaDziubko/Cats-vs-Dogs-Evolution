using _Game.Core._DataLoaders.EnvironmentDataLoader;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._Battle.Scripts;
using _Game.UI._Environment;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.EnvironmentDataProvider
{
    public class EnvironmentDataLoader : IEnvironmentDataLoader
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public EnvironmentDataLoader(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        
        public async UniTask<EnvironmentData> Load(string key, LoadContext context)
        {
            await _assetRegistry.Warmup<GameObject>(key);
            
            GameObject environmentGO =
                await _assetRegistry.LoadAsset<GameObject>(key, context.Timeline, context.CacheContext);
            
            _logger.Log($"Environment with key {key} load successfully");
            
            environmentGO.TryGetComponent(out BattleEnvironment prefab);
            
            var data = new EnvironmentData()
            {
                Key = key,
                Prefab = prefab,
            };

            return data;
        }
    }
}