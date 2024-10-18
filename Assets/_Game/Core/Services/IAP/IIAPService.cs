using System;
using System.Collections.Generic;
using _Game.UI._Shop.Scripts._GemsBundle;
using _Game.UI._Shop.Scripts._SpeedOffer;
using UnityEngine.Purchasing;


namespace _Game.Core.Services.IAP
{
    public interface IIAPService
    {
        event Action<SpeedOffer> SpeedOfferRemoved;
        event Action<Product> Purchased;
        event Action Initialized;
        bool IsInitialized { get; }

        void StartPurchase(string productId);
        List<GemsBundle> GemsBundles();
        List<SpeedOffer> SpeedOffers();
        List<ProfitOffer> ProfitOffers();
        void UpdateProducts();
    }

}