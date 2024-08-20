using _Game.Core.Services.Analytics;

namespace _Game.Gameplay._Battle.Scripts
{
    public class BattleAnalyticsController
    {
        private readonly IAnalyticsService _analytics;

        private BattleAnalyticsData _battleAnalyticsData;

        public BattleAnalyticsController(
            IAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public void SendStartData()
        {
            _analytics.OnBattleStarted(_battleAnalyticsData);
        }

        public void UpdateData(BattleAnalyticsData battleAnalyticsData)
        {
            _battleAnalyticsData = battleAnalyticsData;
        }

        public void SendWave(string wave)
        {
            _analytics.SendWave(wave, _battleAnalyticsData);
        }
    }
}