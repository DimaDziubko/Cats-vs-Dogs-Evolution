using System;
using Assets._Game.Core.UserState;
using Assets._Game.Utils;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;
        public float Gems;

        public event Action<bool> CoinsChanged;
        public event Action GemsChanged;

        float IUserCurrenciesStateReadonly.Coins => Coins + Constants.ComparisonThreshold.MONEY_EPSILON;
        float IUserCurrenciesStateReadonly.Gems => Gems;

        public void ChangeCoins(in float delta, bool isPositive)
        {
            Coins += delta;
            
            if (Coins < 0) Coins = 0;
            
            CoinsChanged?.Invoke(isPositive);
        }
        
        public void ChangeGems(in float delta)
        {
            Gems += delta;
            GemsChanged?.Invoke();
        }

        public void RemoveAllCoins()
        {
            Coins = 0;
            CoinsChanged?.Invoke(false);
        }
    }
}