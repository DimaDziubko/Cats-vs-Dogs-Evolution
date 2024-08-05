using System;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Hud._BattleSpeedView;
using Assets._Game.Gameplay._Timer.Scripts;

namespace _Game.Core.Services._BattleSpeedService._Scripts
{
    public interface IBattleSpeedService
    {
        event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        event Action<GameTimer, bool> SpeedBoostTimerActivityChanged;
        void OnBattleSpeedBtnClicked();
        void OnBattleSpeedBtnShown();
    }
}