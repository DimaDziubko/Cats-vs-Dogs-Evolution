using _Game.Core._DataPresenters.WeaponDataPresenter;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI._Environment.Factory;
using Assets._Game.Core.Factory;
using Assets._Game.Gameplay._Bases.Factory;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.BattleMode
{
    public class FactoriesInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private BaseFactory baseFactory;
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
            var unitDataPresenter = Container.Resolve<IUnitDataProvider>();
            var weaponDataPresenter = Container.Resolve<IWeaponDataProvider>();
            var towerDataPresenter = Container.Resolve<IBaseDataProvider>();
            
            var cameraService = Container.Resolve<IWorldCameraService>();
            var random = Container.Resolve<IRandomService>();
            var audioService = Container.Resolve<IAudioService>();
            var soundService = Container.Resolve<ISoundService>();

            BindProjectileFactory(soundService, weaponDataPresenter);
            BindUnitFactory(cameraService, random, soundService, unitDataPresenter);
            BindTowerFactory(towerDataPresenter, cameraService);
            BindCoinFactory(audioService);
            BindVfxFactory(weaponDataPresenter);
            BindEnvironmentFactory(cameraService);
        }

        private void BindProjectileFactory(
            ISoundService soundService,
            IWeaponDataProvider weaponDataProvider)
        {
            _projectileFactory.Initialize(soundService, weaponDataProvider);
            Container.Bind<IProjectileFactory>().To<ProjectileFactory>().FromInstance(_projectileFactory).AsSingle();
        }

        private void BindUnitFactory(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService,
            IUnitDataProvider unitDataProvider)
        {
            _unitFactory.Initialize(cameraService, random, soundService, unitDataProvider);
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }

        private void BindCoinFactory(IAudioService audioService)
        {
            _coinFactory.Construct(audioService);
            Container.Bind<ICoinFactory>().To<CoinFactory>().FromInstance(_coinFactory).AsSingle();
        }

        private void BindVfxFactory(
            IWeaponDataProvider weaponDataProvider)
        {
            _vfxFactory.Initialize(weaponDataProvider);
            Container.Bind<IVfxFactory>().To<VfxFactory>().FromInstance(_vfxFactory).AsSingle();
        }

        private void BindTowerFactory(
            IBaseDataProvider baseDataProvider,
            IWorldCameraService cameraService)
        {
            baseFactory.Initialize(baseDataProvider, cameraService);
            Container.Bind<IBaseFactory>().To<BaseFactory>().FromInstance(baseFactory).AsSingle();
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

        private void BindFactoriesHolder() => Container.BindInterfacesAndSelfTo<FactoriesHolder>().AsSingle();
    }
}