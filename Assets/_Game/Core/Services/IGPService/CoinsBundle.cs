using System;
using _Game.Core.Configs.Models;
using _Game.UI._Shop.Scripts._ShopScr;

namespace _Game.Core.Services.IGPService
{
    public class CoinsBundle : ShopItem
    {
        public event Action<float> QuantityChanged;
        public event Action<bool> IsAffordableChanged;
        
        public int Id;
        public CoinsBundleConfig Config;

        private float _quantity;
        private bool _isAffordable;
        
        public float Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                QuantityChanged?.Invoke(_quantity);
            }
        }

        public bool IsAffordable
        {
            get => _isAffordable;
            set
            {
                _isAffordable = value;
                IsAffordableChanged?.Invoke(_isAffordable);
            }
        }

        public override int ShopItemViewId => Config.ShopItemViewId;
    }
}