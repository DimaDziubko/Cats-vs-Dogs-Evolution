using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.Services.IGPService
{
    public interface IIGPService
    {
        event Action<IGPDto> Purchased;
        List<ProductDescription> Products();
        void StartPurchase(ProductConfig productDescriptionConfig);
    }
}