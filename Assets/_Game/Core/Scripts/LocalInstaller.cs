using _Game.Bundles.Bases.Factory;
using _Game.Bundles.Units.Common.Factory;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class LocalInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private BaseFactory _baseFactory;
        public override void InstallBindings()
        {
            BindFactories();
        }

        private void BindFactories()
        {
            var battleState = Container.Resolve<IBattleStateService>();
            var ageState = Container.Resolve<IAgeStateService>();
            BindUnitFactory(battleState, ageState);
            BindBaseFactory(battleState, ageState);
        }

        private void BindUnitFactory(IBattleStateService battleState, 
            IAgeStateService ageState)
        {
            _unitFactory.Initialize(battleState, ageState);
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }

        private void BindBaseFactory(IBattleStateService battleState, 
            IAgeStateService ageState)
        {
            _baseFactory.Initialize(battleState, ageState);
            Container.Bind<IBaseFactory>().To<BaseFactory>().FromInstance(_baseFactory).AsSingle();
        }
    }
}
