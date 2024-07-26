using System;
using System.Collections.Generic;

namespace _Game.UI._Shop.Scripts
{
    public interface IShopPresenter
    {
        event Action<List<ShopItemModel>> ShopItemsUpdated;
        void OnShopOpened();
    }
}