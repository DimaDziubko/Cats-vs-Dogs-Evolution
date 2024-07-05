using System.Collections.Generic;
using Assets._Game.Core.Services.AssetProvider;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.AssetManagement
{
    public class AssetRegistry : IAssetRegistry
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<int, HashSet<string>> _contextKeys = new Dictionary<int, HashSet<string>>();
        
        public AssetRegistry(IAssetProvider assetProvider) {
            _assetProvider = assetProvider;
        }

        public async UniTask<T> LoadAsset<T>(string key, int context) where T : class {
            var asset = await _assetProvider.Load<T>(key);
            if (!_contextKeys.ContainsKey(context)) {
                _contextKeys[context] = new HashSet<string>();
            }
            _contextKeys[context].Add(key);
            return asset;
        }

        public void ClearContext(int context) {
            if (_contextKeys.TryGetValue(context, out var keys)) {
                foreach (var key in keys) {
                    _assetProvider.Release(key);
                }
                keys.Clear();
                _contextKeys.Remove(context);
            }
        }
        
    }
}