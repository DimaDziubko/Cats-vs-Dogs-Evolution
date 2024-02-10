using _Game.Bundles.Units.Common.Factory;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class LocalInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        public override void InstallBindings()
        {
            BindUnitFactory();
        }

        private void BindUnitFactory()
        {
            _unitFactory.Initialize(Container.Resolve<IBattleStateService>());
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }
    }
}
