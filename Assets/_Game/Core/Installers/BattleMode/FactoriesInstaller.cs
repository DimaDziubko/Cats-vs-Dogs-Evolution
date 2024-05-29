using _Game.Core.Factory;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI._Environment.Factory;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class FactoriesInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private BaseFactory _baseFactory;
        [SerializeField] private ProjectileFactory _projectileFactory;
        [SerializeField] private CoinFactory _coinFactory;
        [SerializeField] private VfxFactory _vfxFactory;
        [SerializeField] private EnvironmentFactory _environmentFactory;
        
        public override void InstallBindings()
        {
            BindFactories();
            BindFactoriesHolder();
        }
        private void BindFactories()
        {
            var battleState = Container.Resolve<IBattleStateService>();
            var ageState = Container.Resolve<IAgeStateService>();
            var cameraService = Container.Resolve<IWorldCameraService>();
            var random = Container.Resolve<IRandomService>();
            var audioService = Container.Resolve<IAudioService>();

            BindProjectileFactory(battleState, ageState, audioService);
            BindUnitFactory(battleState, ageState, cameraService, random, audioService);
            BindBaseFactory(battleState, ageState, cameraService);
            BindCoinFactory(audioService);
            BindVfxFactory(battleState, ageState);
            BindEnvironmentFactory(cameraService);
        }

        private void BindCoinFactory(IAudioService audioService)
        {
            _coinFactory.Construct(audioService);
            Container.Bind<ICoinFactory>().To<CoinFactory>().FromInstance(_coinFactory).AsSingle();
        }

        private void BindVfxFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState)
        {
            _vfxFactory.Initialize(battleState, ageState);
            Container.Bind<IVfxFactory>().To<VfxFactory>().FromInstance(_vfxFactory).AsSingle();
        }

        private void BindProjectileFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IAudioService audioService)
        {
            _projectileFactory.Initialize(battleState, ageState, audioService);
            Container.Bind<IProjectileFactory>().To<ProjectileFactory>().FromInstance(_projectileFactory).AsSingle();
        }

        private void BindUnitFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IWorldCameraService cameraService,
            IRandomService random,
            IAudioService audioService)
        {
            _unitFactory.Initialize(battleState, ageState, cameraService, random, audioService);
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }

        private void BindBaseFactory(
            IBattleStateService battleState, 
            IAgeStateService ageState,
            IWorldCameraService cameraService)
        {
            _baseFactory.Initialize(battleState, ageState, cameraService);
            Container.Bind<IBaseFactory>().To<BaseFactory>().FromInstance(_baseFactory).AsSingle();
        }


        private void BindEnvironmentFactory(IWorldCameraService cameraService)
        {
            _environmentFactory.Initialize(cameraService);
            Container
                .Bind<IEnvironmentFactory>()
                .To<EnvironmentFactory>()
                .FromInstance(_environmentFactory)
                .AsSingle();
        }

        private void BindFactoriesHolder()
        {
            Container.BindInterfacesAndSelfTo<FactoriesHolder>().AsSingle();
        }
    }
}