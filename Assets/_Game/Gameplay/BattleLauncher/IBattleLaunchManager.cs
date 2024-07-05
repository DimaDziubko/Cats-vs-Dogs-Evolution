namespace Assets._Game.Gameplay.BattleLauncher
{
    public interface IBattleLaunchManager
    {
        void Register(IBattleLauncher handler);
        void TriggerLaunchBattle();
        void UnRegister(IBattleLauncher handler);
    }
}