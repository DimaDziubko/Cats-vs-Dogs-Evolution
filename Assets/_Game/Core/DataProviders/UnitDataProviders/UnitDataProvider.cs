﻿using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.DataProviders.UnitBuilderDataProvider;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.UnitDataProviders
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IEconomyConfigRepository _economyConfigRepository;

        public UnitDataProvider(
            IAssetRegistry assetRegistry,
            IEconomyConfigRepository economyConfigRepository)
        {
            _assetRegistry = assetRegistry;
            _economyConfigRepository = economyConfigRepository;
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

            var go = await _assetRegistry.LoadAsset<GameObject>(unitKey, options.CacheContext);

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