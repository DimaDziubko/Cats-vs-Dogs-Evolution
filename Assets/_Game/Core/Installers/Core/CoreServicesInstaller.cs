using _Game._AssetProvider;
using _Game.Common;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core._StateFactory;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Data;
using _Game.Core.Debugger;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
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
            BindIAPProvider();
            BindIAPService();
            BindIGPService();
            BindFreeGemsPackService();
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

        private void BindStaticDataService() =>
            Container.BindInterfacesAndSelfTo<AssetProvider>()
                .AsSingle().NonLazy();

        private void BindAssetRegistry() => 
            Container.BindInterfacesAndSelfTo<AssetRegistry>()
                .AsSingle();

        private void BindPersistentData() =>
            Container.BindInterfacesAndSelfTo<UserContainer>()
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

        private void BindIAPProvider() =>
            Container
                .Bind<IAPProvider>()
                .AsSingle();

        private void BindIAPService() =>
            Container
                .BindInterfacesAndSelfTo<IAPService>()
                .AsSingle();

        private void BindIGPService() =>
            Container
                .BindInterfacesAndSelfTo<IGPService>()
                .AsSingle();
        private void BindFreeGemsPackService() => 
            Container
                .BindInterfacesAndSelfTo<FreeGemsPackService>()
                .AsSingle();
    }
}