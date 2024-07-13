using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Facade;
using Assets._Game.Core._Logger;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Ambience
{
    public class AmbienceDataProvider : IAmbienceDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public AmbienceDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        public UniTask<AudioClip> Load(string key, LoadContext cacheContext)
        {
            _logger.Log($"Ambience with key {key} loading...");
            return _assetRegistry.LoadAsset<AudioClip>(key, cacheContext.Timeline, cacheContext.CacheContext );
        }
    }
}