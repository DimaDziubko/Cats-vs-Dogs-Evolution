using _Game.Core._GameListenerComposite;

namespace _Game.Gameplay._BattleSpeed.Scripts
{
    public interface IBattleSpeedManager
    {
        float CurrentSpeedFactor { get; }
        void Register(IBattleSpeedListener listener);
        void Unregister(IBattleSpeedListener listener);
        void SetSpeedFactor(float speedFactor);
    }
}