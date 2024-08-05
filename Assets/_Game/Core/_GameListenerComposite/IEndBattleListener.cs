using Assets._Game.Gameplay.GameResult.Scripts;

namespace _Game.Core._GameListenerComposite
{
    public interface IEndBattleListener : IBattleListener
    {
        void OnEndBattle(GameResultType result, bool wasExit);
    }
}