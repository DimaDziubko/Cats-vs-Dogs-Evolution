using Assets._Game.Core.AssetManagement;
using Assets._Game.Gameplay.Battle.Scripts;
using Assets._Game.UI._Environment;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.EnvironmentDataProvider
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