using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders._FoodDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.DataPresenters.WeaponDataPresenter;
using _Game.Core.DataProviders.Common;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataProvidersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitDataProvider();
            BindCommonItemsDataProvider();
            BindBaseDataProvider();
            BindWeaponDataProvider();
            BindFoodProductionDataProvider();
        }

        private void BindUnitDataProvider() => 
            Container.BindInterfacesAndSelfTo<UnitDataProvider>()
                .AsSingle();

        private void BindCommonItemsDataProvider() =>
            Container.BindInterfacesAndSelfTo<CommonItemsDataProvider>()
                .AsSingle();

        private void BindBaseDataProvider() =>
            Container
                .BindInterfacesAndSelfTo<BaseDataProvider>()
                .AsSingle();
        
        private void BindWeaponDataProvider() =>
            Container
                .BindInterfacesAndSelfTo<WeaponDataProvider>()
                .AsSingle();
        private void BindFoodProductionDataProvider() => 
            Container.BindInterfacesAndSelfTo<FoodProductionDataProvider>()
                .AsSingle();
    }
}