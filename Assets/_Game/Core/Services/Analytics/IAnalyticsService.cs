using Assets._Game.Gameplay.Battle.Scripts;

namespace Assets._Game.Core.Services.Analytics
{
    public interface IAnalyticsService
    {
        void OnBattleStarted(BattleAnalyticsData battleAnalyticsData);
        void SendEvent(string mainMenu);
    }
}