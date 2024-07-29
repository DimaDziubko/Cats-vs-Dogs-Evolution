using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._BattleSpeed
{
    public class BattleSpeedStateHandler : IBattleSpeedStateHandler
    {
        private readonly IUserContainer _userContainer;

        public BattleSpeedStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void ChangeNormalSpeed(bool isNormal) => 
            _userContainer.State.BattleSpeedState.SetNormalSpeedActive(isNormal);

        public void ChangeBattleSpeedTimerDurationLeft(float timerTimeLeft) => 
            _userContainer.State.BattleSpeedState.ChangeDurationLeft(timerTimeLeft);

        public void ChangePermanentSpeedId(int newId) => 
            _userContainer.State.BattleSpeedState.ChangePermanentSpeed(newId);
    }
}