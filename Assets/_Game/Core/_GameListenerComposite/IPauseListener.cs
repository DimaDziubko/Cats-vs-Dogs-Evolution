namespace _Game.Core._GameListenerComposite
{
    public interface IPauseListener : IBattleListener
    {
        void SetPaused(bool isPaused);
    }
}