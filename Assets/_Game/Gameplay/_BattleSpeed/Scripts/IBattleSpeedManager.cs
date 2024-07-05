namespace Assets._Game.Gameplay._BattleSpeed.Scripts
{
    public interface IBattleSpeedManager
    {
        float CurrentSpeedFactor { get; }
        void Register(IBattleSpeedHandler handler);
        public void UnRegister(IBattleSpeedHandler handler);
        void SetSpeedFactor(float speedFactor);
    }
}