using _Game.Audio.Scripts;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Providers;
using _Game.Core.Loading;
using _Game.Core.Pause.Scripts;
using _Game.Core.Prefabs;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.Gameplay.GamePlayManager;
using _Game.Gameplay.UpgradesAndEvolution.Scripts;
using _Game.StaticData;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Popups;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace _Game.Core.Scripts
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Camera _uICameraOverlay;
        [SerializeField] private SFXSourcesHolder _sfxSourcesHolder;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SoundsHolder _soundsHolder;

        //PersistentUI
        [SerializeField] private Header _header;

        public override void InstallBindings()
        {
            BindLogger();
            
            BindSceneLoader();
            BindStaticDataService();
            BindPersistentData();
            BindStateCommunicator();
            BindCameraService();
            BindAudioService();
            BindAlertPopupProvider();
            BindShopPopupProvider();
            BindLoadingScreenProvider();
            BindRandomService();
            BindAddressableAssetProvider();
            BindPauseManager();
            BindISettingsPopupProvider();
            BindStateFactory();


            BindHeader();

            BindBeginGameManager();

            BindConfigProviders();

            BindConfigControllers();

            BindBattleState();
            BindUpgradesAndEvolutionService();

            BindStartBattleWindowProvider();
            BindUpgradeAndEvolutionWindowProvider();
            BindMainMenuProvider();
        }

        private void BindLogger()
        {
            Container
                .BindInterfacesAndSelfTo<MyLogger>()
                .AsSingle();
        }

        private void BindUpgradesAndEvolutionService()
        {
            Container
                .BindInterfacesAndSelfTo<UpgradesAndEvolutionService>()
                .AsSingle();
        }

        private void BindBattleState()
        {
            Container
                .BindInterfacesAndSelfTo<BattleStateService>()
                .AsSingle();
        }
        
        private void BindConfigControllers()
        {
            Container
                .BindInterfacesAndSelfTo<GameConfigController>()
                .AsSingle();
        }

        private void BindConfigProviders()
        {
            Container
                .BindInterfacesAndSelfTo<RemoteConfigProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<LocalConfigProvider>()
                .AsSingle();
        }

        private void BindBeginGameManager()
        {
            Container
                .BindInterfacesAndSelfTo<BeginGameManager>()
                .AsSingle();
        }

        private void BindHeader()
        {
            Container
                .Bind<IHeader>()
                .FromInstance(_header)
                .AsSingle(); 
        }

        private void BindMainMenuProvider()
        {
            Container
                .BindInterfacesAndSelfTo<MainMenuProvider>()
                .AsSingle();
        }

        private void BindStartBattleWindowProvider()
        {
            Container
                .BindInterfacesAndSelfTo<UpgradeAndEvolutionWindowProvider>()
                .AsSingle();
        }

        private void BindUpgradeAndEvolutionWindowProvider()
        {
            Container
                .BindInterfacesAndSelfTo<StartBattleWindowProvider>()
                .AsSingle();
        }

        private void BindISettingsPopupProvider()
        {
            Container
                .BindInterfacesAndSelfTo<SettingsPopupProvider>()
                .AsSingle();
        }

        private void BindStateFactory()
        {
            Container.Bind<StateFactory>().FromMethod(ctx => new StateFactory(ctx.Container)).AsSingle();
        }

        private void BindSceneLoader()
        {
            Container.Bind<SceneLoader>().FromNew().AsSingle();
        }

        private void BindPauseManager()
        {
            Container.Bind<IPauseManager>()
                .To<PauseManager>()
                .AsSingle();
        }

        private void BindAddressableAssetProvider()
        {
            Container.Bind<IAddressableAssetProvider>()
                .To<AddressableAssetProvider>()
                .AsSingle();
        }

        private void BindRandomService()
        {
            Container.Bind<IRandomService>()
                .To<UnityRandomService>()
                .AsSingle();
        }

        private void BindLoadingScreenProvider()
        {
            Container.Bind<ILoadingScreenProvider>()
                .To<LoadingScreenProvider>()
                .AsSingle();
        }
        
        private void BindShopPopupProvider()
        {
            Container.Bind<IShopPopupProvider>()
                .To<ShopPopupProvider>()
                .AsSingle();
        }

        private void BindAlertPopupProvider()
        {
            Container.Bind<IAlertPopupProvider>()
                .To<AlertPopupProvider>()
                .AsSingle();
        }

        private void BindAudioService()
        {
            var audioService = new AudioService(
                _audioMixer, 
                _sfxSourcesHolder,
                _musicSource,
                _soundsHolder);
            
            
            Container
                .Bind<IAudioService>()
                .FromInstance(audioService)
                .AsSingle(); 
        }

        private void BindCameraService()
        {
            var cameraService = new WorldCameraService(_uICameraOverlay);
            
            Container
                .Bind<IWorldCameraService>()
                .FromInstance(cameraService)
                .AsSingle();
        }

        private void BindStateCommunicator()
        {
            LocalUserStateCommunicator  communicator =
                new LocalUserStateCommunicator(new JsonSaveLoadStrategy());
            
            Container.Bind<IUserStateCommunicator>()
                .FromInstance(communicator)
                .AsSingle();
        }

        private void BindPersistentData()
        {
            Container.Bind<IPersistentDataService>()
                .To<UserContainer>()
                .AsSingle();
        }

        private void BindStaticDataService()
        {
            AssetProvider assetProvider = new AssetProvider();
            assetProvider.Initialize();
            Container.Bind<IAssetProvider>()
                .FromInstance(assetProvider)
                .AsSingle();
        }
    }
}
