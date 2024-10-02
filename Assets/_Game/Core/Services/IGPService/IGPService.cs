using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core.UserState;
using Zenject;

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
        
        private List<CoinsBundle> _bundles = new List<CoinsBundle>(2);

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
        }

        private void Init()
        {
            Currencies.CurrenciesChanged += OnCurrencyChanged;
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemChanged;
            TimelineState.NextAgeOpened += OnAgeChanged;
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
        
        void IDisposable.Dispose()
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
            foreach (var bundle in _bundles)
            {
                bundle.Quantity = bundle.Config.GetQuantity(TimelineState.AgeId, TimelineState.FoodProductionLevel);
                bundle.IsAffordable = bundle.Config.Price < Currencies.Gems;
            }
        }

        public List<CoinsBundle> CoinsBundles() =>
            GetCoinsBundles().ToList();

        private void Notify(
            string purchaseId,
            string purchaseType,
            int purchaseAmount,
            int purchasePrice,
            string purchaseCurrency,
            Dictionary<string, int> resources = null
        )
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


        private IEnumerable<CoinsBundle> GetCoinsBundles()
        {
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)) yield return null;

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
                
                _bundles.Add(bundle);
                yield return bundle;
            }
        }

        private void ClearBundles() => _bundles.Clear();
    }
}