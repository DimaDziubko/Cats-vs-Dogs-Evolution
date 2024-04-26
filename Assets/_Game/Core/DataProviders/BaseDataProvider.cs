using _Game.Core.AssetManagement;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders
{
    public class BaseDataProvider : IBaseDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public BaseDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }
        
        public async UniTask<BaseData> LoadBase(BaseLoadOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();
            var basePrefab = await _assetRegistry.LoadAsset<GameObject>(options.PrefabKey, options.CacheContext);
            return new BaseData()
            {
                Health = options.Health,
                BasePrefab = basePrefab.GetComponent<Base>(),
                CoinsAmount = options.CoinsAmount,
                Layer = options.Faction == Faction.Player ?  Constants.Layer.PLAYER_BASE : Constants.Layer.ENEMY_BASE,
            };
        }
    }
}