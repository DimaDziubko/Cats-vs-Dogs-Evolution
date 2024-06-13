using _Game.Core.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Ambience
{
    public class AmbienceDataProvider : IAmbienceDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public AmbienceDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }
        public UniTask<AudioClip> Load(string key, int cacheContext) => 
            _assetRegistry.LoadAsset<AudioClip>(key, cacheContext);
    }
}