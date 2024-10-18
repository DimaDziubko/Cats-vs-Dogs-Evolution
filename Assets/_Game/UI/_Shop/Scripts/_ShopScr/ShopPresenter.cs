using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop.Scripts._AdsGemsPack;
using _Game.UI._Shop.Scripts._CoinBundles;
using _Game.UI._Shop.Scripts._FreeGemsPack;
using _Game.UI._Shop.Scripts._GemsBundle;
using _Game.UI._Shop.Scripts._ProfitOffer;
using _Game.UI._Shop.Scripts._SpeedOffer;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopPresenter : IShopPresenter, IDisposable, IUpgradeAvailabilityProvider
    {
        public IEnumerable<GameScreen> AffectedScreens
        {
            get { yield return GameScreen.Shop; }
        }

        public bool IsAvailable => _coinsBundleManager.IsAnyItemAvailable() ||
                                   _freeGemsPackManager.IsAnyItemAvailable() ||
                                   _adsGemsPackManager.IsAnyItemAvailable();

        private readonly IIGPService _igpService;
        private readonly IIAPService _iapService;
        private readonly IAdsGemsPackService _adsGemsPackService;
        private readonly IFreeGemsPackService _freeGemsPackService;

        private readonly IGameInitializer _gameInitializer;
        private readonly IUpgradesAvailabilityChecker _checker;
        private readonly IMyLogger _logger;

        private readonly ShopItemManager<CoinsBundle, CoinsBundleView, CoinsBundlePresenter> _coinsBundleManager;
        private readonly ShopItemManager<GemsBundle, GemsBundleView, GemsBundlePresenter> _gemsBundleManager;
        private readonly ShopItemManager<SpeedOffer, SpeedOfferView, SpeedOfferPresenter> _speedOfferManager;
        private readonly ShopItemManager<ProfitOffer, ProfitOfferView, ProfitOfferPresenter> _profitOfferManager;
        private readonly ShopItemManager<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter> _adsGemsPackManager;
        private readonly ShopItemManager<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter> _freeGemsPackManager;

        public Shop Shop { get; set; }

        public ShopPresenter(
            IIGPService igpService,
            IIAPService iapService,
            IGameInitializer gameInitializer,
            IAdsGemsPackService adsGemsPackService,
            IFreeGemsPackService freeGemsPackService,
            IUpgradesAvailabilityChecker checker,
            IMyLogger logger,
            CoinsBundlePresenter.Factory coinsBundlePresenterFactory,
            GemsBundlePresenter.Factory gemsBundlePresenterFactory,
            SpeedOfferPresenter.Factory speedOfferPresenterFactory,
            ProfitOfferPresenter.Factory profitOfferPresenterFactory,
            AdsGemsPackPresenter.Factory adsGemsPackPresenterFactory,
            FreeGemsPackPresenter.Factory freeGemsPackPresenterFactory)
        {
            _igpService = igpService;
            _iapService = iapService;
            _adsGemsPackService = adsGemsPackService;
            _freeGemsPackService = freeGemsPackService;
            _checker = checker;
            _logger = logger;
            gameInitializer.OnMainInitialization += Init;
            _gameInitializer = gameInitializer;

            _coinsBundleManager = new ShopItemManager<CoinsBundle, CoinsBundleView, CoinsBundlePresenter>(
                null,
                coinsBundlePresenterFactory,
                bundle => bundle.IsAffordable
            );

            _gemsBundleManager = new ShopItemManager<GemsBundle, GemsBundleView, GemsBundlePresenter>(
                null,
                gemsBundlePresenterFactory,
                bundle => true
            );

            _speedOfferManager = new ShopItemManager<SpeedOffer, SpeedOfferView, SpeedOfferPresenter>(
                null,
                speedOfferPresenterFactory,
                speedOffer => true
            );

            _profitOfferManager = new ShopItemManager<ProfitOffer, ProfitOfferView, ProfitOfferPresenter>(
                null,
                profitOfferPresenterFactory,
                profitOffer => true
            );

            _adsGemsPackManager = new ShopItemManager<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter>(
                null,
                adsGemsPackPresenterFactory,
                adsGemsPack => adsGemsPack.IsReady
            );

            _freeGemsPackManager = new ShopItemManager<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter>(
                null,
                freeGemsPackPresenterFactory,
                freeGemsPack => freeGemsPack.IsReady
            );
        }

        private void Init()
        {
            _checker.Register(this);
            _iapService.SpeedOfferRemoved += OnSpeedOfferRemoved;
        }

        public void Dispose()
        {
            _checker.UnRegister(this);
            _gameInitializer.OnMainInitialization -= Init;
            _iapService.SpeedOfferRemoved -= OnSpeedOfferRemoved;
        }

        private void OnSpeedOfferRemoved(SpeedOffer speedOffer) => 
            _speedOfferManager.RemoveItem(speedOffer);

        public void OnShopOpened()
        {
            _iapService.UpdateProducts();
            UpdateAllItems();
        }
        
        private void UpdateAllItems()
        {
            if (Shop == null) return;
        
            _coinsBundleManager.SetShopContainer(Shop.Container);
            _gemsBundleManager.SetShopContainer(Shop.Container);
            _speedOfferManager.SetShopContainer(Shop.Container);
            _profitOfferManager.SetShopContainer(Shop.Container);
            _adsGemsPackManager.SetShopContainer(Shop.Container);
            _freeGemsPackManager.SetShopContainer(Shop.Container);

            UpdateItems(_iapService.ProfitOffers(), AddProfitOffer);
            UpdateItems(_iapService.SpeedOffers(), AddSpeedOffer);
            UpdateItems(_igpService.CoinsBundles, AddCoinsBundle);
            UpdateItems(_iapService.GemsBundles(), AddGemsBundle);
            UpdateItems(_adsGemsPackService.GetAdsGemsPacks().Values.ToList(), AddAdsGemsPacks);
            UpdateItems(_freeGemsPackService.GetFreeGemsPacks().Values.ToList(), AddFreeGemsPacks);
            UpdateDecorationElements();
        }
        
        private void UpdateItems<T>(List<T> items, Action<T> addItemAction)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                addItemAction(item);
            }
        }
        
        public void OnShopClosed()
        {
            _coinsBundleManager.ClearItems();
            _gemsBundleManager.ClearItems();
            _speedOfferManager.ClearItems();
            _profitOfferManager.ClearItems();
            _adsGemsPackManager.ClearItems();
            _freeGemsPackManager.ClearItems();
        }
        
        private void UpdateDecorationElements() => 
            Shop.Container.UpdateDecorationElements();

        private void AddCoinsBundle(CoinsBundle bundle) => 
            _coinsBundleManager.AddItem(bundle, ShopSubContainer.Middle);

        private void AddGemsBundle(GemsBundle bundle) => 
            _gemsBundleManager.AddItem(bundle, ShopSubContainer.Bottom);

        private void AddSpeedOffer(SpeedOffer offer) => 
            _speedOfferManager.AddItem(offer, ShopSubContainer.Top);

        private void AddProfitOffer(ProfitOffer offer) => 
            _profitOfferManager.AddItem(offer, ShopSubContainer.Top);

        private void AddAdsGemsPacks(AdsGemsPack adsGemsPack) => 
            _adsGemsPackManager.AddItem(adsGemsPack, ShopSubContainer.Bottom);

        private void AddFreeGemsPacks(FreeGemsPack freeGemsPack) => 
            _freeGemsPackManager.AddItem(freeGemsPack, ShopSubContainer.Bottom);
    }
}