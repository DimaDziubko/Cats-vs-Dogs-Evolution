using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.UI._Shop.Scripts
{
    public interface IShopPresenter
    {
        event Action<List<ShopItemModel>> ShopItemsUpdated;
        void OnShopOpened();
        void TryToBuy(ProductDescription productDescription);
    }
}