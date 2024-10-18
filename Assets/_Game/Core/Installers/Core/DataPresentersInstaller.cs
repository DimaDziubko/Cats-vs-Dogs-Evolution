using _Game.Core._DataPresenters.UnitBuilderDataPresenter;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts._AdsGemsPack;
using _Game.UI._Shop.Scripts._CoinBundles;
using _Game.UI._Shop.Scripts._FreeGemsPack;
using _Game.UI._Shop.Scripts._GemsBundle;
using _Game.UI._Shop.Scripts._ProfitOffer;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts._SpeedOffer;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataPresentersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindUnitBuilderDataPresenter();
            BindBattlePresenter();
            BindCoinsBundlePresenterFactory();
            BindGemsBundlePresenterFactory();
            BindSpeedOfferPresenterFactory();
            BindProfitOfferPresenterFactory();
            BindAdsGemsPackPresenterFactory();
            BindFreeGemsPackPresenterFactory();
            BindShopPresenter();
            BindMiniShopPresenter();
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

        private void BindCoinsBundlePresenterFactory() =>
            Container
                .BindFactory<CoinsBundle, CoinsBundleView, CoinsBundlePresenter, CoinsBundlePresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindGemsBundlePresenterFactory() =>
            Container
                .BindFactory<GemsBundle, GemsBundleView, GemsBundlePresenter, GemsBundlePresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindSpeedOfferPresenterFactory() =>
            Container
                .BindFactory<SpeedOffer, SpeedOfferView, SpeedOfferPresenter, SpeedOfferPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindProfitOfferPresenterFactory() =>
            Container
                .BindFactory<ProfitOffer, ProfitOfferView, ProfitOfferPresenter, ProfitOfferPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindAdsGemsPackPresenterFactory() =>
            Container
                .BindFactory<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter, AdsGemsPackPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindFreeGemsPackPresenterFactory() =>
            Container
                .BindFactory<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter, FreeGemsPackPresenter.Factory>()
                .AsSingle()
                .NonLazy();

        private void BindShopPresenter() =>
            Container
                .BindInterfacesAndSelfTo<ShopPresenter>()
                .AsSingle();

        private void BindMiniShopPresenter() =>
            Container
                .BindInterfacesAndSelfTo<MiniShopPresenter>()
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