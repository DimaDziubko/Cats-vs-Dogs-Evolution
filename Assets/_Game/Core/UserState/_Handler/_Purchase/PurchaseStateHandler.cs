using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Currencies;
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

        public void PurchaseUnit(UnitType type, float price, CurrenciesSource source)
        {
            if (_userContainer.State.Currencies.Coins >= price)
            {
                _userContainer.State.Currencies.ChangeCoins(price, false, source);
                _userContainer.State.TimelineState.OpenUnit(type);
                _userContainer.RequestSaveGame();

            }
        }

        public void PurchaseCoinsWithGems(float quantity, int price, CurrenciesSource source)
        {
            _userContainer.State.Currencies.ChangeCoins(quantity, true, CurrenciesSource.MiniShop);
            _userContainer.State.Currencies.ChangeGems(price, false, source);
            _userContainer.RequestSaveGame();

        }

        public void AddPurchase(string id)
        {
            _userContainer.State.PurchaseDataState.AddPurchase(id);
            _userContainer.RequestSaveGame();

        }
    }
}