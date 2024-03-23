using System;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;
        public float Gems;
        
        public event Action<bool> CoinsChanged;
        public event Action GemsChanged;

        float IUserCurrenciesStateReadonly.Coins => Coins;
        float IUserCurrenciesStateReadonly.Gems => Gems;

        public void ChangeCoins(in float delta, bool isPositive)
        {
            Coins += delta;
            CoinsChanged?.Invoke(isPositive);
        }
        
        public void ChangeGems(in float delta)
        {
            Coins += delta;
            GemsChanged?.Invoke();
        }
    }
}