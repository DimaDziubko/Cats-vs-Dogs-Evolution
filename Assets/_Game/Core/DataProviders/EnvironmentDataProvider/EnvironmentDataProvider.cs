using _Game.Core.AssetManagement;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._Environment;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.EnvironmentDataProvider
{
    public class EnvironmentDataProvider : IEnvironmentDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public EnvironmentDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<EnvironmentData> Load(string key, int cacheContext)
        {
            GameObject environmentGO =
                await _assetRegistry.LoadAsset<GameObject>(key, cacheContext);

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