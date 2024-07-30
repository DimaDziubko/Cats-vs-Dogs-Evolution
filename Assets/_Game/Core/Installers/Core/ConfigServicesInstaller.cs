using _Game.Core.Configs.Providers;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Configs.Repositories.Timeline;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class ConfigServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigProviders();
            
            BindCommonConfigRepository();
            BindTimelineConfigRepository();
            BindAgeConfigRepository();
            BindBattleConfigRepository();
            BindEconomyConfigRepository();
            BindShopConfigRepository();
            BindAdsConfigRepository();
        }

        private void BindAdsConfigRepository() => 
            Container
                .BindInterfacesAndSelfTo<AdsConfigRepository>()
                .AsSingle();

        private void BindConfigProviders()
        {
            Container
                .BindInterfacesAndSelfTo<RemoteConfigProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<LocalConfigProvider>()
                .AsSingle();
        }

        private void BindCommonConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<CommonItemsConfigRepository>()
                .AsSingle();


        private void BindEconomyConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<EconomyConfigRepository>()
                .AsSingle();

        private void BindTimelineConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<TimelineConfigRepository>()
                .AsSingle();

        private void BindAgeConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<AgeConfigRepository>()
                .AsSingle();

        private void BindBattleConfigRepository() =>
            Container
                .BindInterfacesAndSelfTo<BattleSpeedConfigRepository>()
                .AsSingle();

        private void BindShopConfigRepository()
        {
            Container
                .BindInterfacesAndSelfTo<ShopConfigRepository>()
                .AsSingle();
        }
    }
}