namespace _Game.Core._GameListenerComposite
{
    public interface IBattleSpeedListener : IBattleListener
    {
        void OnBattleSpeedFactorChanged(float speedFactor);
    }
}