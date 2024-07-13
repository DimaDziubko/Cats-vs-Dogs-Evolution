using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.UnitDataProviders;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.UnitUpgradeDataProvider
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public UnitDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask<UnitData> LoadUnitData(UnitLoadOptions options)
        {
            string unitKey;
            
            switch (options.Faction)
            {
                case Faction.Player:
                    unitKey = options.Config.GetUnitKeyForCurrentRace(options.CurrentRace);
                    break;
                case Faction.Enemy:
                    unitKey = options.Config.GetUnitKeyForAnotherRace(options.CurrentRace);
                    break;
                default:
                    unitKey = options.Config.GetUnitKeyForCurrentRace(options.CurrentRace);
                    break;
            }

            var go = await _assetRegistry.LoadAsset<GameObject>(unitKey, options.Timeline, options.CacheContext);

            _logger.Log($"Unit with id {options.Config.Id} load successfully");
            
            return new UnitData
            {
                Config = options.Config,
                Prefab = go.GetComponent<Unit>(),
                UnitLayer = options.Config.GetUnitLayerForFaction(options.Faction),
                AggroLayer = options.Config.GetAggroLayerForFaction(options.Faction),
                AttackLayer = options.Config.GetAttackLayerForFaction(options.Faction),
            };
        }
    }
}