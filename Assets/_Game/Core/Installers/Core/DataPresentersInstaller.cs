using _Game.Core._DataPresenters.UnitBuilderDataPresenter;
using _Game.Core.DataPresenters.BattlePresenter;
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
            BindBattlePresenter();
            BindShopPresenter();
            BindBoostDataPresenter();
            BindCardsPresenter();
            BindCardsScreenPresenter();
        }

        private void BindUnitBuilderDataPresenter() => 
            Container
                .BindInterfacesAndSelfTo<UnitBuilderDataPresenter>()
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
            Container.BindInterfacesAndSelfTo<CardsPresenter>().AsSingle();

        private void BindCardsScreenPresenter() =>
            Container
                .BindInterfacesAndSelfTo<CardsScreenPresenter>()
                .AsSingle();

        private void BindBoostDataPresenter() =>
            Container
                .BindInterfacesAndSelfTo<BoostDataPresenter>()
                .AsSingle();
    }
}