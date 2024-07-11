using System;

namespace _Game.Core.UserState
{
    public class FoodBoostState : IFoodBoostStateReadonly
    {
        public int DailyFoodBoostCount;
        public DateTime LastDailyFoodBoost;

        public event Action FoodBoostChanged;

        int IFoodBoostStateReadonly.DailyFoodBoostCount => DailyFoodBoostCount;
        DateTime IFoodBoostStateReadonly.LastDailyFoodBoost => LastDailyFoodBoost;

        public void ChangeFoodBoostCount(int delta, DateTime lastDailyFoodBoost)
        {
            DailyFoodBoostCount += delta;
            LastDailyFoodBoost = lastDailyFoodBoost;
            
            FoodBoostChanged?.Invoke();
        }
    }

    public interface IFoodBoostStateReadonly
    {
        event Action FoodBoostChanged;
        int DailyFoodBoostCount { get; }
        DateTime LastDailyFoodBoost { get;}
    }
}