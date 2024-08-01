using _Game.Common;
using _Game.UI._Currencies;

namespace _Game.Core.UserState._Handler.Currencies
{
    public interface ICurrenciesHandler
    {
        void AddCoins(in float quantity, CurrenciesSource source);
        void AddGems(in float quantity, CurrenciesSource source);
    }
}