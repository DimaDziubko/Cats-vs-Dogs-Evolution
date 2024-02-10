using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Factory;
using _Game.Core.Services.Battle;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        private IBattleStateService _battleState;
        public void Initialize(IBattleStateService battleState)
        {
            _battleState = battleState;
        }
            
        public Unit GetForPlayer(UnitType type)
        {
            var playerUnitData = _battleState.GetPlayerUnit(type);
            Unit instance = CreateGameObjectInstance(playerUnitData.Prefab);
            
            instance.OriginFactory = this;
            
            instance.Construct(playerUnitData.Config);
            
            return instance;
        }
        
        public Unit GetForEnemy(UnitType type)
        {
            var enemyData = _battleState.GetEnemy(type);
            
            Unit instance = CreateGameObjectInstance(enemyData.Prefab);
            
            instance.OriginFactory = this;
            
            instance.Construct(enemyData.Config);
            
            return instance;
        }

        public void Reclaim(Unit item)
        { 
            Destroy(item.gameObject);
        }
    }
}