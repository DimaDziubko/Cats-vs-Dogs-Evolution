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
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts._SpeedOffer;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.UI._Shop.Scripts
{
    public class ShopPresenter : IShopPresenter, IDisposable, IUpgradeAvailabilityProvider
    {
        public IEnumerable<GameScreen> AffectedScreens
        {
            get { yield return GameScreen.Shop; }
        }
        public bool IsAvailable => _coinsBundlePresenters.Keys
                                       .Any(x => x.IsAffordable) ||
                                   _freeGemsPackPresenters.Keys
                                       .Any(x => x.IsReady);

        private readonly IIGPService _igpService;
        private readonly IIAPService _iapService;
        private readonly IAdsGemsPackService _adsGemsPackService;
        private readonly IFreeGemsPackService _freeGemsPackService;

        private readonly IGameInitializer _gameInitializer;
        private readonly IUpgradesAvailabilityChecker _checker;
        private readonly IMyLogger _logger;

        private readonly CoinsBundlePresenter.Factory _coinsBundlePresenterFactory;
        private readonly GemsBundlePresenter.Factory _gemsBundlePresenterFactory;
        private readonly SpeedOfferPresenter.Factory _speedOfferPresenterFactory;
        private readonly ProfitOfferPresenter.Factory _profitOfferPresenterFactory;
        private readonly AdsGemsPackPresenter.Factory _adsGemsPackPresenterFactory;
        private readonly FreeGemsPackPresenter.Factory _freeGemsPackPresenterFactory;


        private readonly Dictionary<CoinsBundle, CoinsBundlePresenter> _coinsBundlePresenters
            = new Dictionary<CoinsBundle, CoinsBundlePresenter>();

        private readonly Dictionary<GemsBundle, GemsBundlePresenter> _gemsBundlePresenters
            = new Dictionary<GemsBundle, GemsBundlePresenter>();

        private readonly Dictionary<SpeedOffer, SpeedOfferPresenter> _speedOfferPresenters
            = new Dictionary<SpeedOffer, SpeedOfferPresenter>();

        private readonly Dictionary<ProfitOffer, ProfitOfferPresenter> _profitOfferPresenters
            = new Dictionary<ProfitOffer, ProfitOfferPresenter>();

        private readonly Dictionary<AdsGemsPack, AdsGemsPackPresenter> _adsGemsPackPresenters
            = new Dictionary<AdsGemsPack, AdsGemsPackPresenter>();

        private readonly Dictionary<FreeGemsPack, FreeGemsPackPresenter> _freeGemsPackPresenters
            = new Dictionary<FreeGemsPack, FreeGemsPackPresenter>();


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
            _coinsBundlePresenterFactory = coinsBundlePresenterFactory;
            _gemsBundlePresenterFactory = gemsBundlePresenterFactory;
            _speedOfferPresenterFactory = speedOfferPresenterFactory;
            _profitOfferPresenterFactory = profitOfferPresenterFactory;
            _adsGemsPackPresenterFactory = adsGemsPackPresenterFactory;
            _freeGemsPackPresenterFactory = freeGemsPackPresenterFactory;
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

        private void OnSpeedOfferRemoved(SpeedOffer speedOffer)
        {
            if (_speedOfferPresenters.Remove(speedOffer, out SpeedOfferPresenter presenter))
            {
                presenter.Dispose();
                Shop.Container.Remove(presenter.View);
            }
        }

        public void OnShopOpened()
        {
            _iapService.Refresh();
            UpdateItems();
        }

        public void OnShopClosed()
        {
            ClearCoinsBundles();
            ClearGemsBundles();
            ClearSpeedOffers();
            ClearProfitOffers();
            ClearAdsGemsPacks();
            ClearFreeGemsPacks();
        }

        private void ClearFreeGemsPacks()
        {
            foreach (var pair in _freeGemsPackPresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _freeGemsPackPresenters.Clear();
        }

        private void ClearAdsGemsPacks()
        {
            foreach (var pair in _adsGemsPackPresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _adsGemsPackPresenters.Clear();
        }

        private void ClearSpeedOffers()
        {
            foreach (var pair in _speedOfferPresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _speedOfferPresenters.Clear();
        }

        private void ClearGemsBundles()
        {
            foreach (var pair in _gemsBundlePresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _gemsBundlePresenters.Clear();
        }

        private void ClearCoinsBundles()
        {
            foreach (var pair in _coinsBundlePresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _coinsBundlePresenters.Clear();
        }

        private void ClearProfitOffers()
        {
            foreach (var pair in _profitOfferPresenters)
            {
                pair.Value.Dispose();
                Shop.Container.Remove(pair.Value.View);
            }

            _profitOfferPresenters.Clear();
        }

        private void UpdateItems()
        {
            if (Shop == null) return;
            UpdateProfitOffers();
            UpdateSpeedOffers();
            UpdateCoinsBundles();
            UpdateGemsBundles();
            UpdateAdsGemsPacks();
            UpdateFreeGemsPacks();
            UpdateDecorationElements();
        }

        private void UpdateDecorationElements()
        {
            Shop.Container.UpdateDecorationElements();
        }

        private void UpdateAdsGemsPacks()
        {
            Dictionary<int, AdsGemsPack> adsGemsPacks = _adsGemsPackService.GetAdsGemsPacks();
            
            if(adsGemsPacks == null) return;

            foreach (var adsGemsPack in adsGemsPacks.Values)
            {
                AddAdsGemsPacks(adsGemsPack);
            }
        }

        private void UpdateFreeGemsPacks()
        {
            Dictionary<int, FreeGemsPack> freeGemsPacks = _freeGemsPackService.GetFreeGemsPacks();
            
            if(freeGemsPacks == null) return;

            foreach (var freeGemsPack in freeGemsPacks.Values)
            {
                AddFreeGemsPacks(freeGemsPack);
            }
        }

        private void UpdateProfitOffers()
        {
            List<ProfitOffer> offers = _iapService.ProfitOffers();
            
            if(offers == null) return;
            
            foreach (var offer in offers)
            {
                AddProfitOffer(offer);
            }
        }

        private void UpdateSpeedOffers()
        {
            List<SpeedOffer> offers = _iapService.SpeedOffers();

            _logger.Log($"Speed offers is null {offers == null}", DebugStatus.Warning);
            
            if(offers == null) return;

            foreach (var offer in offers)
            {
                _logger.Log($"Speed offer is null {offers == null}", DebugStatus.Warning);
                
                AddSpeedOffer(offer);
            }
        }

        private void UpdateGemsBundles()
        {
            List<GemsBundle> bundles = _iapService.GemsBundles();
            
            if(bundles == null) return;

            foreach (var bundle in bundles)
            {
                AddGemsBundle(bundle);
            }
        }

        private void UpdateCoinsBundles()
        {
            List<CoinsBundle> bundles = _igpService.CoinsBundles();
            
            if(bundles == null) return;

            foreach (var bundle in bundles)
            {
                AddCoinsBundle(bundle);
            }
        }

        private void AddCoinsBundle(CoinsBundle bundle)
        {
            if (!_coinsBundlePresenters.ContainsKey(bundle))
            {
                CoinsBundleView view = Shop.Container.SpawnCoinBundleView(bundle.Config.ShopItemViewId);
                CoinsBundlePresenter presenter = _coinsBundlePresenterFactory.Create(bundle, view);
                _coinsBundlePresenters.Add(bundle, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        private void AddGemsBundle(GemsBundle bundle)
        {
            if (!_gemsBundlePresenters.ContainsKey(bundle))
            {
                GemsBundleView view = Shop.Container.SpawnGemsBundleView(bundle.Config.ShopItemViewId);
                GemsBundlePresenter presenter = _gemsBundlePresenterFactory.Create(bundle, view);
                _gemsBundlePresenters.Add(bundle, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        private void AddSpeedOffer(SpeedOffer offer)
        {
            if (!_speedOfferPresenters.ContainsKey(offer))
            {
                SpeedOfferView view = Shop.Container.SpawnSpeedOfferView(offer.Config.ShopItemViewId);
                SpeedOfferPresenter presenter = _speedOfferPresenterFactory.Create(offer, view);
                _speedOfferPresenters.Add(offer, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        private void AddProfitOffer(ProfitOffer offer)
        {
            if (!_profitOfferPresenters.ContainsKey(offer))
            {
                ProfitOfferView view = Shop.Container.SpawnProfitOfferView(offer.Config.ShopItemViewId);
                ProfitOfferPresenter presenter = _profitOfferPresenterFactory.Create(offer, view);
                _profitOfferPresenters.Add(offer, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        private void AddAdsGemsPacks(AdsGemsPack adsGemsPack)
        {
            if (!_adsGemsPackPresenters.ContainsKey(adsGemsPack))
            {
                AdsGemsPackView view = Shop.Container.SpawnAdsGemsPackView(adsGemsPack.Config.ShopItemViewId);
                AdsGemsPackPresenter presenter = _adsGemsPackPresenterFactory.Create(adsGemsPack, view);
                _adsGemsPackPresenters.Add(adsGemsPack, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        private void AddFreeGemsPacks(FreeGemsPack freeGemsPack)
        {
            if (!_freeGemsPackPresenters.ContainsKey(freeGemsPack))
            {
                FreeGemsPackView view = Shop.Container.SpawnFreeGemsPackView(freeGemsPack.Config.ShopItemViewId);
                FreeGemsPackPresenter presenter = _freeGemsPackPresenterFactory.Create(freeGemsPack, view);
                _freeGemsPackPresenters.Add(freeGemsPack, presenter);
                presenter.Initialize();
                view.Init();
            }
        }
    }
}