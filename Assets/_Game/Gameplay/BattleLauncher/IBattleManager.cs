using _Game.Core._GameListenerComposite;
using Assets._Game.Gameplay.GameResult.Scripts;

namespace _Game.Gameplay.BattleLauncher
{
    public interface IBattleManager
    {
        BattleState State { get; }
        bool IsPaused { get;}
        void StartBattle();
        void Register(IBattleListener listener);
        void Unregister(IBattleListener listener);
        void SetPaused(bool isPaused);
        void StopBattle();
        void EndBattle(GameResultType result, bool wasExit = false);
    }
}