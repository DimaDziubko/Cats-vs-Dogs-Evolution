using _Game.Core.Services.UserContainer;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.UserState._Handler._Purchase
{
    public class PurchaseStateHandler : IPurchaseStateHandler
    {
        private readonly IUserContainer _userContainer;

        public PurchaseStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void PurchaseUnit(UnitType type, float price)
        {
            if (_userContainer.State.Currencies.Coins >= price)
            {
                _userContainer.State.Currencies.ChangeCoins(price, false);
                _userContainer.State.TimelineState.OpenUnit(type);
            }
        }

        public void PurchaseCoinsWithGems(int quantity, int price)
        {
            _userContainer.State.Currencies.ChangeCoins(quantity, true);
            _userContainer.State.Currencies.ChangeGems(price, false);
        }

        public void AddPurchase(string id) => 
            _userContainer.State.PurchaseDataState.AddPurchase(id);
    }
}