namespace _Game.Gameplay.BattleLauncher
{
    public sealed  class BattleLaunchManager : IBattleLaunchManager
    {
        private IBattleLauncher _handler;

        public void Register(IBattleLauncher handler)
        {
            if (_handler == null)
                _handler = handler;
        }

        public void UnRegister(IBattleLauncher handler)
        {
            _handler = null;
        }

        public void TriggerLaunchBattle()
        {
           _handler.LaunchBattle();
        }
    }
}