namespace _Game.UI._MainMenu.State
{
    public interface IMenuPayloadedState<TPayload> : IMenuExitableState
    {
        void Enter(TPayload destination);
    }
}