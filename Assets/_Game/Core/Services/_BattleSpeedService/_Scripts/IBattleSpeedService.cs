using System;
using _Game.UI._SpeedBoostBtn.Scripts;
using Assets._Game.Gameplay._Timer.Scripts;

namespace Assets._Game.Core.Services._BattleSpeedService._Scripts
{
    public interface IBattleSpeedService
    {
        event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        event Action<GameTimer, bool> SpeedBoostTimerActivityChanged;
        void OnBattleSpeedBtnClicked(BattleSpeedBtnState state);
        void OnBattleSpeedBtnShown();
        void OnBattleStarted();
        void OnBattleStopped();
        void OnBattlePaused(bool isPaused);
    }
}