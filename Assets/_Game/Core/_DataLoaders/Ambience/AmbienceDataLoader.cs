using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Ambience;
using _Game.Core.DataProviders.Facade;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.Ambience
{
    public class AmbienceDataLoader : IAmbienceDataLoader
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public AmbienceDataLoader(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        public async UniTask<AudioClip> Load(string key, LoadContext cacheContext)
        {
            await _assetRegistry.Warmup<AudioClip>(key);
            _logger.Log($"Ambience with key {key} loading...");
            return await _assetRegistry.LoadAsset<AudioClip>(key, cacheContext.Timeline, cacheContext.CacheContext);
        }
    }
}