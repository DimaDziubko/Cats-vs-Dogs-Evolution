using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.UserState._Handler._Timeline
{
    public interface ITimelineStateHandler
    {
        void OpenNewAge(bool isNext = true);
        void OpenNewTimeline(bool isNext = true);
        void OpenNewBattle(int nextBattle);
        void ChooseRace(Race race);
        void SetAllBattlesWon(bool allBattlesWon);
    }
}