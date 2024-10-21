using _Game.Core.Configs.Models;
using _Game.UI._Shop.Scripts._ShopScr;
using UnityEngine.Purchasing;

namespace _Game.UI._Shop.Scripts._GemsBundle
{
    public class GemsBundle : ShopItem
    {
        public string Id;
        public Product Product;
        public GemsBundleConfig Config;
        public int AvailablePurchasesLeft;
        public override int ShopItemViewId => Config.ShopItemViewId;
    }
}