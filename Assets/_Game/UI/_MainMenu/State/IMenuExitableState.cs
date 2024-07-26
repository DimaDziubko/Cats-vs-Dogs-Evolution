namespace _Game.UI._MainMenu.State
{
    public interface IMenuExitableState
    {
        void Enter();
        void Exit();
        void Cleanup();
    }
}