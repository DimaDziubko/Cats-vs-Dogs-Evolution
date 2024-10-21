using System;
using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core.UserState;

namespace _Game.Core.Services.IGPService
{
    public class IGPDto
    {
        public string PurchaseId;
        public string PurchaseType;
        public int PurchaseAmount;
        public int PurchasePrice;
        public string PurchaseCurrency;
        public Dictionary<string, int> Resources;
    }

    public class IGPService : IIGPService, IDisposable
    {
        public event Action<IGPDto> Purchased;

        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;

        private List<CoinsBundle> _bundlesCache;

        public List<CoinsBundle> CoinsBundles => _bundlesCache;

        public IGPService(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _shopConfigRepository = configRepositoryFacade.ShopConfigRepository;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;

            _gameInitializer.OnMainInitialization += Init;
            _bundlesCache = new List<CoinsBundle>();
        }

        private void Init()
        {
            Currencies.CurrenciesChanged += OnCurrencyChanged;
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemChanged;
            TimelineState.NextAgeOpened += OnAgeChanged;

            UpdateCoinBundles();
        }

        public void StartPurchase(CoinsBundle bundle)
        {
            _userContainer.PurchaseStateHandler
                .PurchaseCoinsWithGems(bundle.Quantity, bundle.Config.Price, CurrenciesSource.Shop);

            Notify(
                bundle.Config.IGP_ID,
                CurrencyType.Coins.ToString(),
                (int) bundle.Quantity,
                bundle.Config.Price,
                CurrencyType.Gems.ToString());
        }

        public void Dispose()
        {
            Currencies.CurrenciesChanged -= OnCurrencyChanged;
            TimelineState.UpgradeItemLevelChanged -= OnUpgradeItemChanged;
            TimelineState.NextAgeOpened -= OnAgeChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnAgeChanged() => UpdateCoinBundles();


        private void OnUpgradeItemChanged(UpgradeItemType type, int _)
        {
            if (type == UpgradeItemType.FoodProduction)
            {
                UpdateCoinBundles();
            }
        }

        private void OnCurrencyChanged(CurrencyType type, double __, CurrenciesSource ___)
        {
            if (type == CurrencyType.Gems)
            {
                UpdateCoinBundles();
            }
        }

        private void UpdateCoinBundles()
        {
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)) return;

            ClearBundles();

            var configs = _shopConfigRepository.GetCoinsBundleConfigs();

            foreach (var config in configs)
            {
                var bundle = new CoinsBundle()
                {
                    Id = config.Id,
                    Config = config,
                    Quantity = config.GetQuantity(TimelineState.AgeId, TimelineState.FoodProductionLevel),
                    IsAffordable = config.Price < Currencies.Gems
                };

                _bundlesCache.Add(bundle);
            }
        }

        private void ClearBundles() => _bundlesCache.Clear();

        private void Notify(
            string purchaseId,
            string purchaseType,
            int purchaseAmount,
            int purchasePrice,
            string purchaseCurrency,
            Dictionary<string, int> resources = null)
        {
            var dto = new IGPDto()
            {
                PurchaseId = purchaseId,
                PurchaseType = purchaseType,
                PurchaseAmount = purchaseAmount,
                PurchasePrice = purchasePrice,
                PurchaseCurrency = purchaseCurrency,
                Resources = resources
            };
            Purchased?.Invoke(dto);
        }
    }
}