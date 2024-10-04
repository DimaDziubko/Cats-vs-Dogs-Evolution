using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._Currencies;
using _Game.UI._Shop.Scripts;
using _Game.Utils;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPService : IIAPService, IDisposable
    {
        private const int INFINITY_PURCHASES_TRIGGER = -1;
        private readonly string PUBLIC_GOOGLE_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxGde1T1Vx9a9V7BaMET88bAzmTHVpx9NKxrsezjORO+cmKHlFGHe1LshZ4E/OcJN3FAPqTakWEV0xh5uINsnFWDfSzu0BnSFHVuhZ8D3bmUGoEqItB8yCqBA8HYLKo+645d5uCO6L4HhEJvAgcUEKIFOpFCqj+kgbp5klUbVWIsbmDsLB7JKM0cu5TSJmnOXSlldmUkeOMQNmy+chnkLFN9+ZY9jz6x2rntuv2w9g1cXgq9n84v4aQX5z2gosbLum3tAg3HbRYophyKw8BTDC98vitT1X1JTjcxDjYvokBDO6YOc0ALHqjb0YU+J6rIvBpOW39YL5V69u7Zm0owCvwIDAQAB";


        public event Action<SpeedOffer> SpeedOfferRemoved;
        public event Action Initialized;
        public event Action<Product> Purchased;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAPProvider _iapProvider;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IMyLogger _logger;

        private IPurchaseDataStateReadonly PurchaseData => _userContainer.State.PurchaseDataState;

        public bool IsInitialized => _iapProvider.IsInitialized;
        public string PublicGooglePlayKey => PUBLIC_GOOGLE_KEY;


        private List<GemsBundle> _gemsBundlesCache;
        private List<SpeedOffer> _speedOffersCache;
        private List<ProfitOffer> _profitOffersCache;

        public IAPService(
            IAPProvider iapProvider,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IFeatureUnlockSystem featureUnlockSystem,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _iapProvider = iapProvider;
            _gameInitializer = gameInitializer;
            _featureUnlockSystem = featureUnlockSystem;
            _logger = logger;

            _gameInitializer.OnPreInitialization += Init;
        }

        private void Init()
        {
            _iapProvider.Initialize(this);
            _iapProvider.Initialized += () => Initialized?.Invoke();
            PurchaseData.Changed += OnPurchaseDataChanged;

            _gemsBundlesCache = new List<GemsBundle>();
            _speedOffersCache = new List<SpeedOffer>();
            _profitOffersCache = new List<ProfitOffer>();

            UpdateCaches();
        }

        public void Dispose()
        {
            _gameInitializer.OnPreInitialization -= Init;
            PurchaseData.Changed -= OnPurchaseDataChanged;
        }

        public List<GemsBundle> GemsBundles() => _gemsBundlesCache;

        public List<SpeedOffer> SpeedOffers() => _speedOffersCache;

        public List<ProfitOffer> ProfitOffers() => _profitOffersCache;

        private void ClearCaches()
        {
            _gemsBundlesCache.Clear();
            _speedOffersCache.Clear();
            _profitOffersCache.Clear();
        }

        private void UpdateCaches()
        {
            ClearCaches();

            foreach (var bundle in GemsBundleDefinitions())
            {
                _gemsBundlesCache.Add(bundle);
            }

            foreach (var offer in SpeedOffersDefinition())
            {
                if (offer != null)
                {
                    _speedOffersCache.Add(offer);
                }
            }

            foreach (var offer in ProfitOffersDefinition())
            {
                _profitOffersCache.Add(offer);
            }
        }

        private void UpdateSpeedOffersCache()
        {
            _speedOffersCache.Clear();
            foreach (var offer in SpeedOffersDefinition())
            {
                if (offer != null)
                {
                    _speedOffersCache.Add(offer);
                }
            }
        }

        private void OnPurchaseDataChanged()
        {
            _logger.Log("OnPurchaseDataChanged", DebugStatus.Success);

            UpdateCaches();
            CheckSpeedOffersBoughtOut();
            CheckProfitOffers();
        }

        private IEnumerable<GemsBundle> GemsBundleDefinitions()
        {
            foreach (string productId in _iapProvider.GemsBundleProducts.Keys)
            {
                GemsBundleConfig config = _iapProvider.GemsBundleConfigs[productId];
                Product product = _iapProvider.GemsBundleProducts[productId];

                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == productId);

                if (ProductBoughtOut(boughtIap, config) &&
                    config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER)
                {
                    continue;
                }

                yield return new GemsBundle()
                {
                    Id = productId,
                    Config = config,
                    Product = product,
                    AvailablePurchasesLeft = boughtIap != null
                        ? config.MaxPurchaseCount - boughtIap.Count
                        : config.MaxPurchaseCount,
                };
            }
        }

        private IEnumerable<SpeedOffer> SpeedOffersDefinition()
        {
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.BattleSpeed))
                yield break;

            foreach (string productId in _iapProvider.SpeedOffers.Keys)
            {
                SpeedBoostOfferConfig config = _iapProvider.SpeedBoostOfferConfigs[productId];
                Product product = _iapProvider.SpeedOffers[productId];

                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == productId);

                if (ProductBoughtOut(boughtIap, config) && config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER)
                {
                    continue;
                }

                if (HasRequiredProduct(config) && !HasRequiredProductBought(config))
                {
                    continue;
                }

                yield return new SpeedOffer()
                {
                    Id = productId,
                    Config = config,
                    Product = product,
                };
            }
        }

        private IEnumerable<ProfitOffer> ProfitOffersDefinition()
        {
            foreach (string productId in _iapProvider.ProfitOffers.Keys)
            {
                ProfitOfferConfig config = _iapProvider.ProfitOfferConfigs[productId];
                Product product = _iapProvider.ProfitOffers[productId];

                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == productId);

                var isActive = ProductBoughtOut(boughtIap, config) &&
                               config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER;

                yield return new ProfitOffer()
                {
                    Id = productId,
                    Config = config,
                    Product = product,
                    IsActive = isActive
                };
            }
        }

        private bool HasRequiredProduct(SpeedBoostOfferConfig config) =>
            config.RequiredIdBought != Constants.ConfigKeys.MISSING_KEY;

        private bool HasRequiredProductBought(SpeedBoostOfferConfig config) =>
            PurchaseData.BoughtIAPs.Any(x => x.IAPId == config.RequiredIdBought);

        private bool ProductBoughtOut(BoughtIAP boughtIap, ProductConfig config) =>
            boughtIap != null && boughtIap.Count >= config.MaxPurchaseCount;

        public void StartPurchase(string productId) =>
            _iapProvider.StartPurchase(productId);

        public PurchaseProcessingResult ProcessPurchase(Product purchasedProduct)
        {
            string id = purchasedProduct.definition.id;

            if (_iapProvider.GemsBundleConfigs.TryGetValue(id, out var config))
            {
                HandleGemsBundlePurchase(id, config);
            }
            else if (_iapProvider.SpeedBoostOfferConfigs.TryGetValue(id, out var speedOffer))
            {
                _userContainer.BattleSpeedStateHandler.ChangePermanentSpeedId(speedOffer.BattleSpeed.Id);
                _userContainer.PurchaseStateHandler.AddPurchase(purchasedProduct.definition.id);
            }
            else if (_iapProvider.ProfitOfferConfigs.TryGetValue(id, out var profitOffer))
            {
                foreach (var moneyBox in profitOffer.MoneyBoxes)
                {
                    switch (moneyBox.CurrencyType)
                    {
                        case CurrencyType.Coins:
                            _userContainer.CurrenciesHandler.AddCoins(moneyBox.Quantity, CurrenciesSource.Shop);
                            break;
                        case CurrencyType.Gems:
                            _userContainer.CurrenciesHandler.AddGems(moneyBox.Quantity, CurrenciesSource.Shop);
                            break;
                    }
                }
                _userContainer.PurchaseStateHandler.AddPurchase(purchasedProduct.definition.id);
            }

            Purchased?.Invoke(purchasedProduct);
            return PurchaseProcessingResult.Complete;
        }

        private void HandleGemsBundlePurchase(string id, GemsBundleConfig config)
        {
            _userContainer.CurrenciesHandler.AddGems(config.Quantity, CurrenciesSource.Shop);
            _userContainer.PurchaseStateHandler.AddPurchase(id);
        }

        private void CheckProfitOffers()
        {
            foreach (var offer in _profitOffersCache)
            {
                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == offer.Id);
                if (!offer.IsActive && ProductBoughtOut(boughtIap, offer.Config) &&
                    offer.Config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER)
                {
                    offer.IsActive = true;
                }
            }
        }

        private void CheckSpeedOffersBoughtOut()
        {
            foreach (var offer in _speedOffersCache)
            {
                BoughtIAP boughtIap = PurchaseData.BoughtIAPs.Find(x => x.IAPId == offer.Id);
                if (ProductBoughtOut(boughtIap, offer.Config) &&
                    offer.Config.MaxPurchaseCount != INFINITY_PURCHASES_TRIGGER)
                {
                    SpeedOfferRemoved?.Invoke(offer);
                }
            }
        }
    }
}