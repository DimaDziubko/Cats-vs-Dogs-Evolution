using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;


namespace _Game.Core.Services.IAP
{
    public interface IIAPService
    {
        event Action<Product> Purchased;
        bool IsInitialized { get; }
        event Action Initialized;
        List<ProductDescription> Products();
        void StartPurchase(string productId);
    }
}