using _Game.Bundles.Bases.Scripts;
using _Game.Core.Factory;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using UnityEngine;

namespace _Game.Bundles.Bases.Factory
{
    [CreateAssetMenu(fileName = "Base Factory", menuName = "Factories/Base")]
    public class BaseFactory : GameObjectFactory, IBaseFactory
    {
        private IBattleStateService _battleState;
        private IAgeStateService _ageState;

        public void Initialize(
            IBattleStateService battleState, 
            IAgeStateService ageState)
        {
            _battleState = battleState;
            _ageState = ageState;
        }
        
        public Base GetPlayerBase()
        {
            var baseData = _ageState.GetForPlayerBase();
            
            Base instance = CreateGameObjectInstance(baseData.BasePrefab);
            
            instance.OriginFactory = this;
            
            return instance;
        }

        public Base GetEnemyBase()
        {
            var baseData = _battleState.GetForEnemyBase();
            
            Base instance = CreateGameObjectInstance(baseData.BasePrefab);
            
            instance.OriginFactory = this;
            
            instance.Construct(baseData.Health);
            
            return instance;
        }

        public void Reclaim(Base @base)
        {
            Destroy(@base.gameObject);
        }
    }
}