using System;
using System.Collections.Generic;

namespace _Game.Core.Services.IGPService
{
    public interface IIGPService
    {
        event Action<IGPDto> Purchased;
        List<CoinsBundle> CoinsBundles();
        void StartPurchase(CoinsBundle config);
    }
}