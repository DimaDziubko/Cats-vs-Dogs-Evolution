using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProductDescription
    {
        public string Id;
        public Product Product;
        public ProductConfig Config;
        public int AvailablePurchasesLeft;
        public int MaxPurchasesCount;
        public bool IsReady;
    }
}