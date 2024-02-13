using System;

namespace _Game.Core.UserState
{
    public interface IUserCurrenciesStateReadonly
    {
        float Coins { get; }
        float Gems { get; }
        event Action CoinsChanged;
        event Action GemsChanged;
    }
}