using System;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;
        public float Gems;
        
        public event Action CoinsChanged;
        public event Action GemsChanged;

        float IUserCurrenciesStateReadonly.Coins => Coins;
        float IUserCurrenciesStateReadonly.Gems => Gems;

        public void ChangeCoins(in float delta)
        {
            Coins += delta;
            CoinsChanged?.Invoke();
        }
        
        public void ChangeGems(in float delta)
        {
            Coins += delta;
            GemsChanged?.Invoke();
        }
    }
}