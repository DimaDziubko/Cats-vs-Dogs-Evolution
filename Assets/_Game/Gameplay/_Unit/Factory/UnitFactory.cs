using _Game.Core.Configs.Controllers;
using _Game.Core.Factory;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using _Game.Gameplay._Unit.Scripts;
using _Game.Gameplay.Common;
using UnityEngine;

namespace _Game.Gameplay._Unit.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        private IRandomService _random;
        private IAssetProvider _assetProvider;
        private IBattleStateService _battleState;

        public void Initialize(
            IRandomService random,
            IAssetProvider staticData,
            IBattleStateService battleState)
        {
            _random = random;
            _assetProvider = staticData;
            _battleState = battleState;
        }
            
        public Unit GetForPlayer(UnitType type)
        {
            // var config = _staticData.ForUnit(type);
            //
            // Unit instance = CreateGameObjectInstance(config.PlayerPrefab);
            // instance.OriginFactory = this;
            // instance.Construct();
            
            return null;
        }
        
        public Unit GetForEnemy(int index)
        {
            var config = _battleState.GetEnemyConfig(index);
            var asset = _assetProvider.ForEnemy(index);
            
            Unit instance = CreateGameObjectInstance(asset.EnemyPrefab);
            instance.OriginFactory = this;
            instance.Construct(config);
            
            return null;
        }

        public void Reclaim(Unit item)
        { 
            Destroy(item.gameObject);
        }
    }
}