using _Game.Core.Services.Analytics;

namespace _Game.Gameplay._Battle.Scripts
{
    public class BattleAnalyticsController
    {
        private readonly IAnalyticsService _analytics;
        private readonly IDTDAnalyticsService _dtdAnalytics;

        private BattleAnalyticsData _battleAnalyticsData;

        public BattleAnalyticsController(
            IAnalyticsService analytics,
            IDTDAnalyticsService dtdAnalytics)
        {
            _analytics = analytics;
            _dtdAnalytics = dtdAnalytics;
        }

        public void SendStartData()
        {
            _analytics.OnBattleStarted(_battleAnalyticsData);
            _dtdAnalytics.OnBattleStarted(_battleAnalyticsData);
        }
        
        public void UpdateData(BattleAnalyticsData battleAnalyticsData)
        {
            _battleAnalyticsData = battleAnalyticsData;
        }

        public void SendWave(string wave)
        {
            _dtdAnalytics.SendWave(wave, _battleAnalyticsData);
            _analytics.SendWave(wave, _battleAnalyticsData);
        }
    }
}