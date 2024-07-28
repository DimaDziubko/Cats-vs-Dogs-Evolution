using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core.Data;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._Currencies;
using _Game.Utils;
using Assets._Game.Core.UserState;

namespace _Game.UI._Shop.Scripts
{
    public class ShopPresenter : IShopPresenter, IDisposable
    {
        public event Action<List<ShopItemModel>> ShopItemsUpdated;
        
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IIAPService _iapService;
        private readonly IIGPService _igpService;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;
        
        public ShopPresenter(
            IGeneralDataPool generalDataPool,
            IIAPService iapService,
            IIGPService igpService,
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _generalDataPool = generalDataPool;
            _iapService = iapService;
            _igpService = igpService;
            _userContainer = userContainer;
            gameInitializer.OnPostInitialization += Init;
            _gameInitializer = gameInitializer;
        }

        private void Init()
        {
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            _iapService.Initialized += UpdateItems;
            Purchases.Changed += OnPurchasesChanged;
        }


        public void Dispose()
        {
            _iapService.Initialized -= UpdateItems;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            Purchases.Changed -= OnPurchasesChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnPurchasesChanged() => 
            UpdateItems();

        private void OnCurrenciesChanged(Currencies _, bool __) => 
            UpdateItems();

        public void OnShopOpened() => 
            UpdateItems();

        public void TryToBuy(ProductDescription productDescription)
        {
            if (productDescription.Id != Constants.ConfigKeys.MISSING_KEY)
            {
                _iapService.StartPurchase(productDescription.Id);
            }
            else
            {
                _igpService.StartPurchase(productDescription.Config);
            }
        }

        private void UpdateItems()
        {
            List<ShopItemModel> models = new List<ShopItemModel>();
            UpdateIAPs(models);
            UpdateIGPs(models);
            ShopItemsUpdated?.Invoke(models);
        }

        private void UpdateIGPs(List<ShopItemModel> models)
        {
            List<ProductDescription> products = _igpService.Products();
            foreach (var product in products)
            {
                ShopItemModel model = new ShopItemModel
                {
                    Description = product,
                    CanAfford = product.Config.Price <= Currencies.Gems,
                    ItemStaticData = _generalDataPool.ShopItemStaticDataPool.ForType(product.Config.Id)
                };
                
                models.Add(model);
            }
        }

        private void UpdateIAPs(List<ShopItemModel> models )
        {
            if(!_iapService.IsInitialized) return;
            List<ProductDescription> products = _iapService.Products();
            foreach (var product in products)
            {
                ShopItemModel model = new ShopItemModel
                {
                    Description = product,
                    CanAfford = true,
                    ItemStaticData = _generalDataPool.ShopItemStaticDataPool.ForType(product.Config.Id)
                };
                
                models.Add(model);
            }
        }
    }
}