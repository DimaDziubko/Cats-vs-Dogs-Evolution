using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.UserState._Handler._Purchase
{
    public interface IPurchaseStateHandler
    {
        void PurchaseUnit(UnitType type, float price);
        void PurchaseCoinsWithGems(int quantity, int price);
        void AddPurchase(string id);
    }
}