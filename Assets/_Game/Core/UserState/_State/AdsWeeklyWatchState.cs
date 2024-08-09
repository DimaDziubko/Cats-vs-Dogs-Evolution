using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public interface IAdsWeeklyWatchStateReadonly
    {
        int LastWeekWatchedAds { get; }
    }

    public class AdsWeeklyWatchState : IAdsWeeklyWatchStateReadonly
    {
        public List<int> LastWeekAdsWatched;
        public DateTime LastDay;
        
        int IAdsWeeklyWatchStateReadonly.LastWeekWatchedAds => LastWeekAdsWatched.Sum();

        public void AddWatchedAd()
        {
            LastWeekAdsWatched[^1]++;
        }

        public void TryChangeDay(DateTime currentDate)
        {
            int daysPassed = (currentDate - LastDay).Days;

            if (daysPassed <= 0)
                return;

            for (int i = 0; i < daysPassed && i < 7; i++)
            {
                LastWeekAdsWatched.RemoveAt(0);
                LastWeekAdsWatched.Add(0);
            }

            LastDay = currentDate;
        }
    }
}