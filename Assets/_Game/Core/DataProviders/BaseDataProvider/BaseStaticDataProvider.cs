using _Game.Core.AssetManagement;
using _Game.Gameplay._Bases.Scripts;
using _Game.Utils;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.BaseDataProvider
{
    public class BaseStaticDataProvider : IBaseStaticDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public BaseStaticDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }
        
        public async UniTask<BaseStaticData> Load(BaseLoadOptions options)
        {
            var basePrefab = await _assetRegistry.LoadAsset<GameObject>(
                options.PrefabKey, 
                options.Timeline, 
                options.CacheContext);
            
            _logger.Log($"Base data with key {options.PrefabKey} load successfully");
            
            return new BaseStaticData()
            {
                BasePrefab = basePrefab.GetComponent<Base>(),
                CoinsAmount = options.CoinsAmount,
                Layer = options.Faction == Faction.Player ?  Constants.Layer.PLAYER_BASE : Constants.Layer.ENEMY_BASE,
            };
        }
    }
}