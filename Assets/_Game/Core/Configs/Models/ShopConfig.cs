using System.Collections.Generic;
using _Game.Core.Services.IAP;
using _Game.UI._Currencies;
using UnityEngine.Purchasing;

namespace _Game.Core.Configs.Models
{
    public class ShopConfig
    {
        public List<GemsBundleConfig> GemsBundleConfigs;
        public List<CoinsBundleConfig> CoinsBundleConfigs;
        public List<SpeedBoostOfferConfig> SpeedBoostOfferConfigs;
        public List<ProfitOfferConfig> ProfitOfferConfigs;
        public List<FreeGemsPackConfig> FreeGemsPackConfigs;
        public List<AdsGemsPackConfig> AdsGemsPackConfigs;
    }

    public class GemsBundleConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public float Quantity;
        public string IAP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public ProductType ProductType;
    }

    public class CoinsBundleConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public int Price;
        public Exponential QuantityExponential;
        public string CurrencyIconKey;
        public string IGP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public int MinishopItemViewId;
        public List<LinearFunction> LinearFunctions;

        public float GetQuantity(int ageId, int foodProductionLevel) => 
            LinearFunctions[ageId].GetIntValue(foodProductionLevel);
    }

    public class SpeedBoostOfferConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string Description;
        public BattleSpeedConfig BattleSpeed;
        public string IAP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public string RequiredIdBought;
        public ProductType ProductType;
    }

    public class AdsGemsPackConfig
    {
        public int Id;
        public int DailyGemsPackCount;
        public int RecoverTimeMinutes;
        public string MajorIconKey;
        public string MinorIconKey;
        public string AdIconKey;
        public int ShopItemViewId;
        public int Quantity;
    }

    public class ProfitOfferConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public string Name;
        public string IAP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public string Description;
        public int CoinBoostFactor;
        public List<MoneyBox> MoneyBoxes;
        public int RequiredIdBought;
        public ProductType ProductType;
    }

    public class MoneyBox
    {
        public int Id;
        public string IconKey;
        public float Quantity;
        internal CurrencyType CurrencyType;
    }
}