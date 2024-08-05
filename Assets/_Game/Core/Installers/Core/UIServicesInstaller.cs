using _Game.Core.LoadingScreen;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._RaceSelectionWindow.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Factory;
using _Game.UI.GameResult.Scripts;
using _Game.UI.Header.Scripts;
using _Game.UI.RateGame.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.Utils.Popups;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class UIServicesInstaller : MonoInstaller
    {
        [SerializeField] private Header _header;
        [SerializeField] private TutorialPointerView _tutorialPointerView;
        [SerializeField] private UIFactory _uiFactory;
        
        public override void InstallBindings()
        {
            BindHeader();
            BindAlertPopupProvider();
            BindTutorialPointerView();
            BindTutorialController();
            BindShopProvider();
            BindMiniShopProvider();
            BindLoadingScreenProvider();
            BindISettingsPopupProvider();
            BindMainMenuProvider();
            BindStartBattleWindowProvider();
            BindUpgradeAndEvolutionWindowProvider();
            BindGameResultWindowProvider();
            BindTimelineInfoWindowProvider();
            BindFactionSelectionWindowProvider();
            BindGameRateScreenProvider();
            BindRateGameChecker();
            BindUIFactory();
        }

        private void BindGameRateScreenProvider() =>
            Container.BindInterfacesTo<RateGameScreenProvider>().AsSingle();

        private void BindHeader() =>
            Container
                .Bind<IHeader>()
                .FromInstance(_header)
                .AsSingle();

        private void BindTutorialPointerView()
        {
            Container
                .Bind<TutorialPointerView>()
                .FromInstance(_tutorialPointerView)
                .AsSingle();
        }

        private void BindTutorialController() => 
            Container.BindInterfacesAndSelfTo<TutorialManager>().AsSingle();


        private void BindAlertPopupProvider() =>
            Container.Bind<IAlertPopupProvider>()
                .To<AlertPopupProvider>()
                .AsSingle();

        private void BindShopProvider() =>
            Container.Bind<IShopProvider>()
                .To<ShopProvider>()
                .AsSingle();

        private void BindMiniShopProvider()
        {
            Container.Bind<IMiniShopProvider>()
                .To<MiniShopProvider>()
                .AsSingle();
        }

        private void BindLoadingScreenProvider() =>
            Container.Bind<ILoadingScreenProvider>()
                .To<LoadingScreenProvider>()
                .AsSingle();

        private void BindISettingsPopupProvider() =>
            Container
                .BindInterfacesAndSelfTo<SettingsPopupProvider>()
                .AsSingle();

        private void BindMainMenuProvider() =>
            Container
                .BindInterfacesAndSelfTo<MainMenuProvider>()
                .AsSingle();

        private void BindStartBattleWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<UpgradeAndEvolutionScreenProvider>()
                .AsSingle();

        private void BindUpgradeAndEvolutionWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<StartBattleScreenProvider>()
                .AsSingle();

        private void BindGameResultWindowProvider() => 
            Container
                .BindInterfacesTo<GameResultPopupProvider>()
                .AsSingle();

        private void BindTimelineInfoWindowProvider() => 
            Container
                .BindInterfacesAndSelfTo<TimelineInfoScreenProvider>()
                .AsSingle();

        private void BindFactionSelectionWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<RaceSelectionWindowProvider>()
                .AsSingle();

        private void BindRateGameChecker() =>
            Container.Bind<IRateGameChecker>().To<RateGameChecker>()
                .AsSingle();

        private void BindUIFactory() => 
            Container.Bind<IUIFactory>().To<UIFactory>().FromInstance(_uiFactory).AsSingle();
    }
}