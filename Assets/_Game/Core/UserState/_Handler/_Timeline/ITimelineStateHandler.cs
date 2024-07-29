using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.UserState._Handler._Timeline
{
    public interface ITimelineStateHandler
    {
        void OpenNextAge();
        void OpenNextTimeline();
        void OpenNextBattle(int nextBattle);
        void ChooseRace(Race race);
        void SetAllBattlesWon(bool allBattlesWon);
    }
}