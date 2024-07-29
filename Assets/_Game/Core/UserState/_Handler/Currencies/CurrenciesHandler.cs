using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler.Currencies
{
    public class CurrenciesHandler : ICurrenciesHandler
    {
        private readonly IUserContainer _userContainer;
        public CurrenciesHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void AddCoins(in float quantity) => 
            _userContainer.State.Currencies.ChangeCoins(quantity, true);

        public void AddGems(in int quantity) => 
            _userContainer.State.Currencies.ChangeGems(quantity, true);
    }
}