using System;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._SpeedBoostBtn.Scripts;

namespace _Game.Core.Services._BattleSpeedService._Scripts
{
    public interface IBattleSpeedService
    {
        void Init();
        event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        event Action<GameTimer, bool> SpeedBoostTimerActivityChanged;
        void OnBattleSpeedBtnClicked(BattleSpeedBtnState state);
        void OnBattleSpeedBtnShown();
        void OnBattleStarted();
        void OnBattleStopped();
        void OnBattlePaused(bool isPaused);
    }
}