using System;
using _Game.UI.Currencies;

namespace _Game.Core.UserState
{
    public interface IUserCurrenciesStateReadonly
    {
        float Coins { get; }
        float Gems { get; }
        event Action<Currencies, bool> CurrenciesChanged;
    }
}