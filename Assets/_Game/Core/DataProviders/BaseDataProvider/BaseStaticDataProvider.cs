using _Game.Core.AssetManagement;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.BaseDataProvider
{
    public class BaseStaticDataProvider : IBaseStaticDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public BaseStaticDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<BaseStaticData> Load(BaseLoadOptions options)
        {
            var basePrefab = await _assetRegistry.LoadAsset<GameObject>(options.PrefabKey, options.CacheContext);
            return new BaseStaticData()
            {
                BasePrefab = basePrefab.GetComponent<Base>(),
                CoinsAmount = options.CoinsAmount,
                Layer = options.Faction == Faction.Player ?  Constants.Layer.PLAYER_BASE : Constants.Layer.ENEMY_BASE,
            };
        }
    }
}