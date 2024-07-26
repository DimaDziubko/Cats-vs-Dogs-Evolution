using System;
using _Game.UI.Currencies;
using _Game.Utils;
using Assets._Game.Core.UserState;
using Assets._Game.Utils;

namespace _Game.Core.UserState
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public float Coins;
        public float Gems;

        public event Action<Currencies, bool> CurrenciesChanged;

        float IUserCurrenciesStateReadonly.Coins => Coins + Constants.ComparisonThreshold.MONEY_EPSILON;
        float IUserCurrenciesStateReadonly.Gems => Gems + Constants.ComparisonThreshold.MONEY_EPSILON;

        public void ChangeCoins(in float delta, bool isPositive)
        {
            Coins += delta;
            
            if (Coins < 0) Coins = 0;
            
            CurrenciesChanged?.Invoke(Currencies.Coins, isPositive);
        }
        
        public void ChangeGems(in float delta, bool isPositive)
        {
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