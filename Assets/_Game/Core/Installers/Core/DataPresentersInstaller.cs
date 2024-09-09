using _Game.Core.DataPresenters._BaseDataPresenter;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Core.DataPresenters.UnitBuilderDataPresenter;
using _Game.Core.DataPresenters.WeaponDataPresenter;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Shop.Scripts;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataPresentersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitBuilderDataPresenter();
            BindWeaponDataPresenter();
            BindTowerDataPresenter();
            BindBattlePresenter();
            BindShopPresenter();
            BindCardsPresenter();
            BindBoostDataPresenter();
        }

        private void BindUnitBuilderDataPresenter() => 
            Container
                .BindInterfacesAndSelfTo<UnitBuilderDataPresenter>()
                .AsSingle();

        private void BindWeaponDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<WeaponDataPresenter>()
                .AsSingle();

        private void BindTowerDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<BasePresenter>()
                .AsSingle();

        private void BindBattlePresenter() =>
            Container
                .BindInterfacesAndSelfTo<BattlePresenter>()
                .AsSingle();

        private void BindShopPresenter() =>
            Container
                .BindInterfacesAndSelfTo<ShopPresenter>()
                .AsSingle();

        private void BindCardsPresenter() =>
            Container
                .BindInterfacesAndSelfTo<CardsScreenPresenter>()
                .AsSingle();

        private void BindBoostDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<BoostDataPresenter>()
                .AsSingle();
    }
}