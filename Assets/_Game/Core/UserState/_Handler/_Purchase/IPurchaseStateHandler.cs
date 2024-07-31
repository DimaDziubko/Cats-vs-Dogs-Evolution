using _Game.UI._Currencies;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.UserState._Handler._Purchase
{
    public interface IPurchaseStateHandler
    {
        void PurchaseUnit(UnitType type, float price, CurrenciesSource source);
        void PurchaseCoinsWithGems(int quantity, int price, CurrenciesSource source);
        void AddPurchase(string id);
    }
}