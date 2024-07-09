﻿using _Game.Core._Logger;
using Assets._Game._AssetProvider;
using Assets._Game.Common;
using Assets._Game.Core._FeatureUnlockSystem.Scripts;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core._GameSaver;
using Assets._Game.Core._Logger;
using Assets._Game.Core._SceneLoader;
using Assets._Game.Core._StateFactory;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Core.Communication;
using Assets._Game.Core.Data;
using Assets._Game.Core.Debugger;
using Assets._Game.Core.Services.AssetProvider;
using Assets._Game.Core.Services.Random;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using UnityEngine;
using Zenject;

namespace Assets._Game.Core.Installers.Core
{
    public class CoreServicesInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private MyDebugger _debugger;

        public override void InstallBindings()
        {
            BindLogger();
            BindDebugger();
            BindInitializer();
            BindSceneLoader();
            BindStaticDataService();
            BindAssetRegistry();
            BindPersistentData();
            BindStateCommunicator();
            BindRandomService();
            BindAddressableAssetProvider();
            BindCoroutineRunner();
            BindStateFactory();
            BindFeatureUnlockSystem();
            BindGameSaver();
            BindDataPool();
        }
        
        private void BindInitializer() =>
            Container
                .BindInterfacesAndSelfTo<GameInitializer>()
                .AsSingle();

        private void BindLogger() =>
            Container
                .BindInterfacesAndSelfTo<MyLogger>()
                .AsSingle();

        private void BindDebugger() =>
            Container
                .BindInterfacesAndSelfTo<MyDebugger>()
                .FromInstance(_debugger)
                .AsSingle();

        private void BindSceneLoader() => 
            Container
                .Bind<SceneLoader>()
                .FromNew()
                .AsSingle();

        private void BindStaticDataService()
        {
            AssetProvider assetProvider = new AssetProvider();
            Container.Bind<IAssetProvider>()
                .FromInstance(assetProvider)
                .AsSingle().NonLazy();
        }

        private void BindAssetRegistry() => 
            Container.BindInterfacesAndSelfTo<AssetRegistry>()
                .AsSingle();

        private void BindPersistentData() =>
            Container.Bind<IUserContainer>()
                .To<UserContainer>()
                .AsSingle();

        private void BindStateCommunicator()
        {
            LocalUserStateCommunicator  communicator =
                new LocalUserStateCommunicator(new JsonSaveLoadStrategy());
            
            Container.Bind<IUserStateCommunicator>()
                .FromInstance(communicator)
                .AsSingle();
        }

        private void BindRandomService() =>
            Container.Bind<IRandomService>()
                .To<UnityRandomService>()
                .AsSingle();

        private void BindAddressableAssetProvider() =>
            Container.Bind<IAddressableAssetProvider>()
                .To<AddressableAssetProvider>()
                .AsSingle();

        private void BindCoroutineRunner() => 
            Container
                .BindInterfacesTo<CoroutineRunner>()
                .FromInstance(_coroutineRunner)
                .AsSingle();

        private void BindStateFactory() => 
            Container.Bind<StateFactory>()
                .FromMethod(ctx => new StateFactory(ctx.Container))
                .AsSingle();

        private void BindFeatureUnlockSystem() =>
            Container
                .BindInterfacesAndSelfTo<FeatureUnlockSystem>()
                .AsSingle();

        private void BindGameSaver() =>
            Container
                .BindInterfacesAndSelfTo<GameSaver>()
                .AsSingle();

        private void BindDataPool() =>
            Container
                .BindInterfacesAndSelfTo<GeneralDataPool>()
                .AsSingle();
    }
}