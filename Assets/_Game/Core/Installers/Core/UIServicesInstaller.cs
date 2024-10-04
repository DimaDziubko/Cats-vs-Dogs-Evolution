using _Game.Core.LoadingScreen;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._AlertPopup;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._RaceSelectionScreen.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.UI.Factory;
using _Game.UI.GameResult.Scripts;
using _Game.UI.Global;
using _Game.UI.Header.Scripts;
using _Game.UI.RateGame.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.TimelineInfoScreen.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class UIServicesInstaller : MonoInstaller
    {
        [SerializeField] private Header _header;
        [SerializeField] private TutorialPointersParent _tutorialPointerParent;
        [SerializeField] private UIFactory _uiFactory;
        [SerializeField] private Curtain _curtain;

        public override void InstallBindings()
        {
            BindUINotifier();
            BindCurtain();
            BindHeader();
            BindAlertPopupProvider();
            BindTutorialPointersParent();
            BindUIFactory();
            BindTutorialManager();
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
            BindCardsScreenProvider();
            BindGeneralCardsScreenProvider();
            BindStatsPopupProvider();
        }

        private void BindUINotifier() => 
            Container.BindInterfacesAndSelfTo<UINotifier>().AsSingle();

        private void BindCurtain() => 
            Container.Bind<Curtain>().FromInstance(_curtain).AsSingle();

        private void BindGameRateScreenProvider() =>
            Container.BindInterfacesTo<RateGameScreenProvider>().AsSingle();

        private void BindHeader() =>
            Container
                .Bind<IHeader>()
                .FromInstance(_header)
                .AsSingle();

        private void BindTutorialPointersParent()
        {
            Container
                .Bind<TutorialPointersParent>()
                .FromInstance(_tutorialPointerParent)
                .AsSingle();
        }
        
        private void BindTutorialManager() => 
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
            Container.BindInterfacesTo<MiniShopProvider>()
                .AsSingle()
                .NonLazy();
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
            Container
            .BindInterfacesAndSelfTo<RateGameChecker>()
                .AsSingle();

        private void BindUIFactory() => 
            Container.BindInterfacesAndSelfTo<UIFactory>().FromInstance(_uiFactory).AsSingle().NonLazy();

        private void BindCardsScreenProvider() =>
            Container.Bind<ICardsScreenProvider>()
                .To<CardsScreenProvider>()
                .AsSingle();

        private void BindGeneralCardsScreenProvider() =>
            Container.Bind<IGeneralCardsScreenProvider>()
                .To<GeneralCardsScreenProvider>()
                .AsSingle();

        private void BindStatsPopupProvider() =>
            Container.Bind<IStatsPopupProvider>()
                .To<StatsPopupProvider>()
                .AsSingle();
        
    }
}