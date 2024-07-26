using System;
using _Game.UI._Currencies;
using _Game.Utils;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;
        public float Gems;

        public event Action<Currencies, bool> CurrenciesChanged;

        float IUserCurrenciesStateReadonly.Coins => Coins + Constants.ComparisonThreshold.MONEY_EPSILON;
        float IUserCurrenciesStateReadonly.Gems => Gems + Constants.ComparisonThreshold.MONEY_EPSILON;

        public void ChangeCoins(float delta, bool isPositive)
        {
            delta = isPositive ? delta : (delta * -1);
            
            Coins += delta;
            
            if (Coins < 0) Coins = 0;
            
            CurrenciesChanged?.Invoke(Currencies.Coins, isPositive);
        }
        
        public void ChangeGems(float delta, bool isPositive)
        {
            delta = isPositive ? delta : (delta * -1);
            
            Gems += delta;
            
            if (Gems < 0) Coins = 0;
            
            CurrenciesChanged?.Invoke(Currencies.Gems, isPositive);
        }

        public void RemoveAllCoins()
        {
            Coins = 0;
            CurrenciesChanged?.Invoke(Currencies.Coins, false);
        }
    }
}