using _Game.Core.Loading;
using _Game.Gameplay.GameResult.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.FactionSelectionWindow.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Popups;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class UIServicesInstaller : MonoInstaller
    {
        [SerializeField] private Header _header;

        public override void InstallBindings()
        {
            BindHeader();
            BindAlertPopupProvider();
            BindShopPopupProvider();
            BindLoadingScreenProvider();
            BindISettingsPopupProvider();
            BindMainMenuProvider();
            BindStartBattleWindowProvider();
            BindUpgradeAndEvolutionWindowProvider();
            BindGameResultWindowProvider();
            BindTimelineInfoWindowProvider();
            BindFactionSelectionWindowProvider();
        }

        private void BindFactionSelectionWindowProvider() =>
            Container
                .BindInterfacesAndSelfTo<FactionSelectionWindowProvider>()
                .AsSingle();

        private void BindHeader() =>
            Container
                .Bind<IHeader>()
                .FromInstance(_header)
                .AsSingle();

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
    }
}