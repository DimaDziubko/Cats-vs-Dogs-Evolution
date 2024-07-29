namespace _Game.Core.UserState._Handler.Currencies
{
    public interface ICurrenciesHandler
    {
        void AddCoins(in float quantity);
        void AddGems(in int quantity);
    }
}