using System.Threading.Tasks;
using _Game.Gameplay.Battle.Scripts;

namespace _Game.Core.Services.Analytics
{
    public interface IDTDAnalyticsService
    {
        void Init();
        void OnBattleStarted(BattleAnalyticsData battleAnalyticsData);
    }
}