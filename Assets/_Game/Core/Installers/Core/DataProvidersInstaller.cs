using Assets._Game.Core.DataProviders.AgeDataProvider;
using Assets._Game.Core.DataProviders.Ambience;
using Assets._Game.Core.DataProviders.BaseDataProvider;
using Assets._Game.Core.DataProviders.BattleDataProvider;
using Assets._Game.Core.DataProviders.Common;
using Assets._Game.Core.DataProviders.EnvironmentDataProvider;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Core.DataProviders.Timeline;
using Assets._Game.Core.DataProviders.UnitBuilderDataProvider;
using Assets._Game.Core.DataProviders.UnitDataProviders;
using Assets._Game.Core.DataProviders.UnitUpgradeDataProvider;
using Assets._Game.Core.DataProviders.WeaponDataProviders;
using Zenject;

namespace Assets._Game.Core.Installers.Core
{
    public class DataProvidersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitDataProvider();
            BindWeaponDataProvider();
            BaseDataProvider();
            
            BindUniversalUnitDataProvider();
            BindUnitBuilderDataProvider();
            BindAgeWeaponDataProvider();
            BindEnvironmentDataProvider();
            BindAmbienceDataProvider();
            BindUnitUpgradeDataProvider();
            BindCommonItemsDataProvider();
            BindTimelineDataProvider();
            
            BindDataProviderFacade();

            BindAgeDataProvider();
            BindBattleDataProvider();
        }

        private void BindTimelineDataProvider() =>
            Container.BindInterfacesAndSelfTo<TimelineDataProvider>()
                .AsSingle();

        private void BindCommonItemsDataProvider() =>
            Container.BindInterfacesAndSelfTo<CommonItemsDataProvider>()
                .AsSingle();

        private void BindUnitDataProvider() => 
            Container.BindInterfacesAndSelfTo<UnitDataProvider>()
                .AsSingle();

        private void BindWeaponDataProvider() => 
            Container.BindInterfacesAndSelfTo<WeaponDataProvider>()
                .AsSingle();

        private void BaseDataProvider() => 
            Container.BindInterfacesAndSelfTo<BaseStaticDataProvider>()
                .AsSingle();

        private void BindUniversalUnitDataProvider() =>
            Container.BindInterfacesAndSelfTo<UniversalUnitDataProvider>()
                .AsSingle();

        private void BindUnitBuilderDataProvider() =>
            Container.BindInterfacesAndSelfTo<UnitBuilderDataProvider>()
                .AsSingle();

        private void BindAgeWeaponDataProvider() =>
            Container.BindInterfacesAndSelfTo<UniversalWeaponDataProvider>()
                .AsSingle();

        private void BindEnvironmentDataProvider() =>
            Container.BindInterfacesAndSelfTo<EnvironmentDataProvider>()
                .AsSingle();

        private void BindAmbienceDataProvider() =>
            Container.BindInterfacesAndSelfTo<AmbienceDataProvider>()
                .AsSingle();

        private void BindUnitUpgradeDataProvider() =>
            Container.BindInterfacesAndSelfTo<UnitUpgradeDataProvider>()
                .AsSingle();

        private void BindDataProviderFacade() =>
            Container.BindInterfacesAndSelfTo<DataProviderFacade>()
                .AsSingle();

        private void BindAgeDataProvider() =>
            Container.BindInterfacesAndSelfTo<AgeDataProvider>()
                .AsSingle();

        private void BindBattleDataProvider() =>
            Container.BindInterfacesAndSelfTo<BattleDataProvider>()
                .AsSingle();
    }
}