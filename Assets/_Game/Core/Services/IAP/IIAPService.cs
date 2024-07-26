using System;
using System.Collections.Generic;

namespace _Game.Core.Services.IAP
{
    public interface IIAPService
    {
        bool IsInitialized { get; }
        event Action Initialized;
        List<ProductDescription> Products();
        void StartPurchase(string productId);
    }
}