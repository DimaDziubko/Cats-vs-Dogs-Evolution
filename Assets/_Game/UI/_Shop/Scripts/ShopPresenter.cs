using System;
using System.Collections.Generic;
using _Game.Core.Data;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using Assets._Game.Core.UserState;

namespace _Game.UI._Shop.Scripts
{
    public class ShopPresenter : IShopPresenter
    {
        public event Action<List<ShopItemModel>> ShopItemsUpdated;
        
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IIAPService _iapService;
        private readonly IIGPService _igpService;
        private readonly IUserContainer _userContainer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;

        public ShopPresenter(
            IGeneralDataPool generalDataPool,
            IIAPService iapService,
            IIGPService igpService,
            IUserContainer userContainer)
        {
            _generalDataPool = generalDataPool;
            _iapService = iapService;
            _igpService = igpService;
            _userContainer = userContainer;
        }
        
        public void OnShopOpened()
        {
            UpdateItems();
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