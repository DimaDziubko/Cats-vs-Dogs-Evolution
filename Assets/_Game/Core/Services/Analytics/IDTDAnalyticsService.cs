using _Game.Gameplay._Battle.Scripts;

namespace Assets._Game.Core.Services.Analytics
{
    public interface IDTDAnalyticsService
    {
        void OnBattleStarted(BattleAnalyticsData battleAnalyticsData);
        void SendEvent(string mainMenu);
    }
}