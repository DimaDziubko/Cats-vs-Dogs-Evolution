using System;

namespace _Game.Core.UserState._State
{
    public class AdsStatistics : IAdsStatisticsReadonly
    {
        public int AdsReviewed;

        public event Action AdsReviewedChanged;

        int IAdsStatisticsReadonly.AdsReviewed => AdsReviewed;

        public void AddWatchedAd()
        {
            AdsReviewed++;    
            AdsReviewedChanged?.Invoke();
        }
    }

    public interface IAdsStatisticsReadonly
    {
        int  AdsReviewed { get; }
        event Action AdsReviewedChanged;
    }
}