using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;

namespace _Game.Core.UserState._Handler.Currencies
{
    public class CurrenciesHandler : ICurrenciesHandler
    {
        private readonly IUserContainer _userContainer;
        public CurrenciesHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void AddCoins(in float quantity, CurrenciesSource source)
        {
            _userContainer.State.Currencies.ChangeCoins(quantity, true, source);
            _userContainer.RequestSaveGame();
        }

        public void AddGems(in float quantity, CurrenciesSource source)
        {
            _userContainer.State.Currencies.ChangeGems(quantity, true, source);
            _userContainer.RequestSaveGame();
        }

        public void SpendGems(in float quantity, CurrenciesSource source)
        {
            _userContainer.State.Currencies.ChangeGems(quantity, false, source);
            _userContainer.RequestSaveGame();
        }

        public void SpendCoins(in float quantity, CurrenciesSource source)
        {
            _userContainer.State.Currencies.ChangeCoins(quantity, false, source);
            _userContainer.RequestSaveGame();
        }
    }
}