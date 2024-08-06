using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.UI.Factory;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProductConfig
    {
        public int Id;
        public string MajorProductIconKey;
        public string MinorProductIconKey;
        public int Price;
        public float Quantity;
        public string Description;
        public ItemType ItemType;
        public string CurrencyIconKey;
        public ProductType ProductType;
        public string IAP_ID;
        public string IGP_ID;
        public int MaxPurchaseCount;
        public ShopItemViewType ItemViewType;
        public List<LinearFunction> LinearFunctions;
    }
}