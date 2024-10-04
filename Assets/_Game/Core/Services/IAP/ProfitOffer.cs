using System;
using _Game.Core.Configs.Models;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProfitOffer
    {
        public event Action<bool> IsActiveChanged;
        
        public string Id;
        public ProfitOfferConfig Config;
        public Product Product;
        
        private bool _isActive;
        
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                IsActiveChanged?.Invoke(IsActive);
            }
        }
    }
}