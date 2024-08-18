using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace _Game.Core.AssetManagement
{
    public class AssetRegistry : IAssetRegistry
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<int, Dictionary<int, HashSet<string>>> _timelineContextKeys 
            = new Dictionary<int, Dictionary<int, HashSet<string>>>();

        private readonly IMyLogger _logger;

        public AssetRegistry(
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _assetProvider = assetProvider;
            _logger = logger;
        }

        public async UniTask<T> LoadAsset<T>(AssetReference assetReference, int timeline, int context) where T : class
        {
            var asset = await _assetProvider.Load<T>(assetReference);

            string cacheKey = assetReference.AssetGUID;

            CacheAsset(timeline, context, cacheKey);

            return asset;
        }
        
        public async UniTask<T> LoadAsset<T>(string key, int timeline, int context) where T : class
        {
            var asset = await _assetProvider.Load<T>(key);

            CacheAsset(timeline, context, key);

            return asset;
        }

        private void CacheAsset(int timeline, int context, string cacheKey)
        {
            if (!_timelineContextKeys.ContainsKey(timeline))
            {
                _timelineContextKeys[timeline] = new Dictionary<int, HashSet<string>>();
            }

            if (!_timelineContextKeys[timeline].ContainsKey(context))
            {
                _timelineContextKeys[timeline][context] = new HashSet<string>();
            }

            _timelineContextKeys[timeline][context].Add(cacheKey);
        }

        public void ClearContext(int timeline, int context)
        {
            if (_timelineContextKeys.TryGetValue(timeline, out var contextKeys)
                && contextKeys.TryGetValue(context, out var keys))
            {
                _logger.Log($"Clearing cache for timeline {timeline}. context {context}");

                foreach (var key in keys)
                {
                    _assetProvider.Release(key);
                }
                keys.Clear();
                contextKeys.Remove(context);

                if (contextKeys.Count == 0)
                {
                    _timelineContextKeys.Remove(timeline);
                }
            }
        }

        public void ClearTimeline(int timeline)
        {
            if (_timelineContextKeys.TryGetValue(timeline, out var contextKeys))
            {
                foreach (var keys in contextKeys.Values)
                {
                    foreach (var key in keys)
                    {
                        _assetProvider.Release(key);
                    }
                    keys.Clear();
                }
                _timelineContextKeys.Remove(timeline);
            }
        }
        
    }
}