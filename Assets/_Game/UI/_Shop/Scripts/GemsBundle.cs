using _Game.Core.Configs.Models;
using UnityEngine.Purchasing;

namespace _Game.UI._Shop.Scripts
{
    public class GemsBundle
    {
        public string Id;
        public Product Product;
        public GemsBundleConfig Config;
        public int AvailablePurchasesLeft;
    }
}