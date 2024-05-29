using _Game.Gameplay.Battle.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Analytics
{
    public interface IAnalyticsService
    {
        UniTask Init();
        void OnBattleStarted(BattleAnalyticsData battleAnalyticsData);
    }
}