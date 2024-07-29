namespace _Game.Core.UserState._Handler._Analytics
{
    public interface IAnalyticsStateHandler
    {
        void AddCompletedBattle();
        void AddAdsReviewed();
        void FirstDayRetentionSent();
        void SecondDayRetentionSent();
    }
}