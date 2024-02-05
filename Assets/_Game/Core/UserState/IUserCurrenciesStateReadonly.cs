using System;

namespace _Game.Core.UserState
{
    public interface IUserCurrenciesStateReadonly
    {
        float Coins { get; }
        event Action Changed;
    }
}