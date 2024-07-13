﻿using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Core.DataPresenters.WeaponDataPresenter;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using Assets._Game.Core.DataPresenters.UnitDataPresenter;
using Assets._Game.Core.DataPresenters.WeaponDataPresenter;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Core.Services.Random;
using Assets._Game.Gameplay._Bases.Factory;
using Assets._Game.Gameplay._Coins.Factory;
using Assets._Game.Gameplay._Units.Factory;
using Assets._Game.UI._Environment.Factory;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Assets._Game.Core.Installers.BattleMode
{
    public class FactoriesInstaller : MonoInstaller
    {
        [SerializeField] private UnitFactory _unitFactory;
        [FormerlySerializedAs("towerFactory")] [FormerlySerializedAs("_baseFactory")] [SerializeField] private BaseFactory baseFactory;
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
            var unitDataPresenter = Container.Resolve<IUnitDataPresenter>();
            var weaponDataPresenter = Container.Resolve<IWeaponDataPresenter>();
            var towerDataPresenter = Container.Resolve<IBasePresenter>();
            
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
            IWeaponDataPresenter weaponDataPresenter)
        {
            _projectileFactory.Initialize(soundService, weaponDataPresenter);
            Container.Bind<IProjectileFactory>().To<ProjectileFactory>().FromInstance(_projectileFactory).AsSingle();
        }

        private void BindUnitFactory(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService,
            IUnitDataPresenter unitDataPresenter)
        {
            _unitFactory.Initialize(cameraService, random, soundService, unitDataPresenter);
            Container.Bind<IUnitFactory>().To<UnitFactory>().FromInstance(_unitFactory).AsSingle();
        }

        private void BindCoinFactory(IAudioService audioService)
        {
            _coinFactory.Construct(audioService);
            Container.Bind<ICoinFactory>().To<CoinFactory>().FromInstance(_coinFactory).AsSingle();
        }

        private void BindVfxFactory(
            IWeaponDataPresenter weaponDataPresenter)
        {
            _vfxFactory.Initialize(weaponDataPresenter);
            Container.Bind<IVfxFactory>().To<VfxFactory>().FromInstance(_vfxFactory).AsSingle();
        }

        private void BindTowerFactory(
            IBasePresenter basePresenter,
            IWorldCameraService cameraService)
        {
            baseFactory.Initialize(basePresenter, cameraService);
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