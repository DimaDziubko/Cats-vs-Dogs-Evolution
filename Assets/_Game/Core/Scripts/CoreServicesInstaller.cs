using _Game._AssetProvider;
using _Game.Common;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.UserState;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class CoreServicesInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;
        public override void InstallBindings()
        {
            BindLogger();
            BindSceneLoader();
            BindStaticDataService();
            BindPersistentData();
            BindStateCommunicator();
            BindRandomService();
            BindAddressableAssetProvider();
            BindCoroutineRunner();
            BindStateFactory();
        }
        
        private void BindLogger() =>
            Container
                .BindInterfacesAndSelfTo<MyLogger>()
                .AsSingle();

        private void BindSceneLoader() => 
            Container
                .Bind<SceneLoader>()
                .FromNew()
                .AsSingle();
        
        private void BindStaticDataService()
        {
            AssetProvider assetProvider = new AssetProvider();
            assetProvider.Init();
            Container.Bind<IAssetProvider>()
                .FromInstance(assetProvider)
                .AsSingle();
        }
        
        private void BindPersistentData() =>
            Container.Bind<IPersistentDataService>()
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
    }
}