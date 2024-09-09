using _Game.Core._DataLoaders.AgeDataProvider;
using _Game.Core._DataLoaders.Ambience;
using _Game.Core._DataLoaders.BaseDataProvider;
using _Game.Core._DataLoaders.BattleDataProvider;
using _Game.Core._DataLoaders.EnvironmentDataProvider;
using _Game.Core._DataLoaders.Facade;
using _Game.Core._DataLoaders.ShopDataProvider;
using _Game.Core._DataLoaders.Timeline;
using _Game.Core._DataLoaders.UnitDataLoaders;
using _Game.Core._DataLoaders.UnitUpgradeDataProvider;
using _Game.Core._DataLoaders.WeaponDataProviders;
using _Game.Core.DataLoaders.UnitBuilderDataProvider;
using _Game.Core.DataLoaders.UnitDataLoaders;
using _Game.Core.DataLoaders.WeaponDataProviders;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataLoadersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitDataLoader();
            BindWeaponDataLoader();
            BaseDataLoader();
            
            BindUniversalUnitDataLoader();
            BindUnitBuilderDataProvider();
            BindAgeWeaponDataProvider();
            BindEnvironmentDataLoader();
            BindAmbienceDataProvider();
            BindUnitUpgradeDataLoader();
            
            BindTimelineDataLoader();
            BindShopDataLoader();
            
            BindDataLoaderFacade();

            BindAgeDataLoader();
            BindBattleDataLoader();
        }

        private void BindTimelineDataLoader() =>
            Container.BindInterfacesAndSelfTo<TimelineDataLoader>()
                .AsSingle();


        private void BindUnitDataLoader() => 
            Container.BindInterfacesAndSelfTo<UnitDataLoader>()
                .AsSingle();

        private void BindWeaponDataLoader() => 
            Container.BindInterfacesAndSelfTo<WeaponDataLoader>()
                .AsSingle();

        private void BaseDataLoader() => 
            Container.BindInterfacesAndSelfTo<BaseStaticDataLoader>()
                .AsSingle();

        private void BindUniversalUnitDataLoader() =>
            Container.BindInterfacesAndSelfTo<UniversalUnitDataLoader>()
                .AsSingle();

        private void BindUnitBuilderDataProvider() =>
            Container.BindInterfacesAndSelfTo<UnitBuilderDataLoader>()
                .AsSingle();

        private void BindAgeWeaponDataProvider() =>
            Container.BindInterfacesAndSelfTo<UniversalWeaponDataLoader>()
                .AsSingle();

        private void BindEnvironmentDataLoader() =>
            Container.BindInterfacesAndSelfTo<EnvironmentDataLoader>()
                .AsSingle();

        private void BindAmbienceDataProvider() =>
            Container.BindInterfacesAndSelfTo<AmbienceDataLoader>()
                .AsSingle();

        private void BindUnitUpgradeDataLoader() =>
            Container.BindInterfacesAndSelfTo<UnitUpgradeDataLoader>()
                .AsSingle();

        private void BindDataLoaderFacade() =>
            Container.BindInterfacesAndSelfTo<DataLoaderFacade>()
                .AsSingle();

        private void BindAgeDataLoader() =>
            Container.BindInterfacesAndSelfTo<AgeDataLoader>()
                .AsSingle();

        private void BindBattleDataLoader() =>
            Container.BindInterfacesAndSelfTo<BattleDataLoader>()
                .AsSingle();

        private void BindShopDataLoader() =>
            Container.BindInterfacesAndSelfTo<ShopDataLoader>()
                .AsSingle();
    }
}