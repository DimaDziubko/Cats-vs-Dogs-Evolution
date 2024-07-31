using System;
using _Game.Common;
using _Game.UI._Currencies;

namespace _Game.Core.UserState._State
{
    public interface IUserCurrenciesStateReadonly
    {
        double Coins { get; }
        double Gems { get; }
        event Action<Currencies, double, CurrenciesSource> CurrenciesChanged;
    }
}