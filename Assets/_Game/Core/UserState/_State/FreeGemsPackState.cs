using System;

namespace _Game.Core.UserState
{
    public class FreeGemsPackState : IFreeGemsPackStateReadonly
    {
        public int FreeGemPackCount;
        public DateTime LastFreeGemPackDay;

        public event Action FreeGemsPackCountChanged;

        int IFreeGemsPackStateReadonly.FreeGemPackCount => FreeGemPackCount;
        DateTime IFreeGemsPackStateReadonly.LastFreeGemPackDay => LastFreeGemPackDay;

        public void ChangeFreeGemPackCount (int delta, DateTime lastDailyFoodBoost)
        {
            FreeGemPackCount += delta;
            LastFreeGemPackDay = lastDailyFoodBoost;
            
            FreeGemsPackCountChanged?.Invoke();
        }
    }
}