using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Gameplay._Bases.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.BaseDataProvider
{
    public class BaseStaticDataLoader : IBaseStaticDataLoader
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public BaseStaticDataLoader(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        
        public async UniTask<BaseStaticData> Load(BaseLoadOptions options)
        {
            await _assetRegistry.Warmup<GameObject>(options.BasePrefab);
            
            var basePrefab = await _assetRegistry.LoadAsset<GameObject>(
                options.BasePrefab, 
                options.Timeline, 
                options.CacheContext);

            return new BaseStaticData()
            {
                BasePrefab = basePrefab.GetComponent<Base>(),
                CoinsAmount = options.CoinsAmount,
                Layer = options.Faction == Faction.Player ?  Constants.Layer.PLAYER_BASE : Constants.Layer.ENEMY_BASE,
            };
        }
    }
}