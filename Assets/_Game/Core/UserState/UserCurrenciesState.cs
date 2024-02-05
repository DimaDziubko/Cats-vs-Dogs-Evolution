using System;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;

        public event Action Changed;

        float IUserCurrenciesStateReadonly.Coins => Coins;

        public void ChangeCoins(in float delta)
        {
            Coins += delta;
            Changed?.Invoke();
        }
    }
}