namespace _Game.UI._MainMenu.State
{
    public interface ILocalExitableState
    {
        void Enter();
        void Exit();
        void Cleanup();
    }
}