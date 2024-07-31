using System;
using _Game.Common;
using _Game.UI._Currencies;
using _Game.Utils;

namespace _Game.Core.UserState._State
{
    public class UserCurrenciesState : IUserCurrenciesStateReadonly
    {
        public double Coins;
        public double Gems;

        public event Action<Currencies, double, CurrenciesSource> CurrenciesChanged;

        double IUserCurrenciesStateReadonly.Coins => Coins + Constants.ComparisonThreshold.MONEY_EPSILON;
        double IUserCurrenciesStateReadonly.Gems => Gems + Constants.ComparisonThreshold.MONEY_EPSILON;

        public void ChangeCoins(float delta, bool isPositive, CurrenciesSource source)
        {
            delta = isPositive ? delta : (delta * -1);
            
            Coins += delta;
            
            if (Coins < 0) Coins = 0;
            
            CurrenciesChanged?.Invoke(Currencies.Coins, delta, source);
        }
        
        public void ChangeGems(float delta, bool isPositive, CurrenciesSource source)
        {
            delta = isPositive ? delta : (delta * -1);
            
            Gems += delta;
            
            if (Gems < 0) Coins = 0;
            
            CurrenciesChanged?.Invoke(Currencies.Gems, delta, source);
        }

        public void RemoveAllCoins()
        {
            var delta = -Coins;
            Coins = 0;
            CurrenciesChanged?.Invoke(Currencies.Coins, delta, CurrenciesSource.None);
        }
    }
}