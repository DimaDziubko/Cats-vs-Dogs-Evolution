using System;

namespace _Game.Core.UserState
{
    public interface IUserCurrenciesStateReadonly
    {
        float Coins { get; }
        float Gems { get; }
        event Action<bool> CoinsChanged;
        event Action GemsChanged;
    }
}