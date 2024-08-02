using _Game.Core.Services.UserContainer;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.UserState._Handler._Timeline
{
    public class TimelineStateHandler : ITimelineStateHandler
    {
        private readonly IUserContainer _userContainer;
        public TimelineStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void OpenNewAge(bool isNext = true)
        {
            _userContainer.State.Currencies.RemoveAllCoins();
            _userContainer.State.TimelineState.OpenNewAge(isNext);
        }

        public void OpenNewTimeline(bool isNext = true)
        {
            _userContainer.State.Currencies.RemoveAllCoins();
            _userContainer.State.TimelineState.OpenNewTimeline(isNext);
        }
        
        public void OpenNewBattle(int nextBattle) => 
            _userContainer.State.TimelineState.OpenNextBattle(nextBattle);
        
        public void ChooseRace(Race race) => 
            _userContainer.State.RaceState.Change(race);
        
        public void SetAllBattlesWon(bool allBattlesWon) => 
            _userContainer.State.TimelineState.SetAllBattlesWon(true);
    }
}