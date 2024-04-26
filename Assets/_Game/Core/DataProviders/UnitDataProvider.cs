using _Game.Core.AssetManagement;
using _Game.Core.Configs.Controllers;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IGameConfigController _gameConfigController;

        public UnitDataProvider(
            IAssetRegistry assetRegistry,
            IGameConfigController gameConfigController)
        {
            _assetRegistry = assetRegistry;
            _gameConfigController = gameConfigController;
        }

        public async UniTask<UnitData> LoadUnitData(UnitLoadOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

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

        public async UniTask<UnitBuilderBtnData> LoadUnitBuilderData(BuilderLoadOptions options)
        {
            var foodIconKey = _gameConfigController.GetFoodIconKey();
            
            options.CancellationToken.ThrowIfCancellationRequested();
            var foodSprite = await _assetRegistry.LoadAsset<Sprite>(foodIconKey, Constants.CacheContext.AGE);
            
            options.CancellationToken.ThrowIfCancellationRequested();
            
            string iconKey = options.Config.GetUnitIconKeyForRace(options.CurrentRace);
            var unitIcon = await _assetRegistry.LoadAsset<Sprite>(iconKey, Constants.CacheContext.AGE);
            
            return new UnitBuilderBtnData
            {
                Type = options.Config.Type,
                FoodIcon = foodSprite,
                UnitIcon = unitIcon,
                FoodPrice = options.Config.FoodPrice,
            };
        }
    }
}