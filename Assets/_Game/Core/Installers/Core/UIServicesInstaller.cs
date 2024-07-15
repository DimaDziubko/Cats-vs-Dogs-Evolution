using _Game.UI.RateGame.Scripts;
using Assets._Game.Core.LoadingScreen;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay.GameResult.Scripts;
using Assets._Game.UI._MainMenu.Scripts;
using Assets._Game.UI._RaceSelectionWindow.Scripts;
using Assets._Game.UI._StartBattleWindow.Scripts;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Settings.Scripts;
using Assets._Game.UI.Shop.Scripts;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Scripts;
using Assets._Game.Utils.Popups;
using UnityEngine;
using Zenject;

namespace Assets._Game.Core.Installers.Core
{
    public class UIServicesInstaller : MonoInstaller
    {
        [SerializeField] private Header _header;
        [SerializeField] private TutorialPointerView _tutorialPointerView;
        
        public override void InstallBindings()
        {
            BindHeader();
            BindAlertPopupProvider();
            BindTutorialPointerView();
            BindTutorialController();
            BindShopPopupProvider();
            BindLoadingScreenProvider();
            BindISettingsPopupProvider();
            BindMainMenuProvider();
            BindStartBattleWindowProvider();
            BindUpgradeAndEvolutionWindowProvider();
            BindGameResultWindowProvider();
            BindTimelineInfoWindowProvider();
            BindFactionSelectionWindowProvider();
            BindGameRateScreenProvider();
        }

        private void BindGameRateScreenProvider() => Container.BindInterfacesTo<RateGameScreenProvider>();

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

        private void BindShopPopupProvider() =>
            Container.Bind<IShopPopupProvider>()
                .To<ShopPopupProvider>()
                .AsSingle();

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
                .BindInterfacesAndSelfTo<UpgradeAndEvolutionWindowProvider>()
                .AsSingle();

        private void BindUpgradeAndEvolutionWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<StartBattleWindowProvider>()
                .AsSingle();

        private void BindGameResultWindowProvider() => 
            Container
                .BindInterfacesTo<GameResultWindowProvider>()
                .AsSingle();

        private void BindTimelineInfoWindowProvider() => 
            Container
                .BindInterfacesAndSelfTo<TimelineInfoWindowProvider>()
                .AsSingle();

        private void BindFactionSelectionWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<RaceSelectionWindowProvider>()
                .AsSingle();
    }
}