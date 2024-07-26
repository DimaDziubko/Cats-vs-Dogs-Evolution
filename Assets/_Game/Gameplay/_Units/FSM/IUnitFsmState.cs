namespace _Game.Gameplay._Units.FSM
{
    public interface IUnitFsmState : IUnitFsmExitableState
    { 
        void Enter();
    }
    
    public interface IUnitFsmExitableState 
    {
        void Exit();
        void GameUpdate();
        void Cleanup();
    }
    
    
    public interface IUnitFsmPayloadedState<TPayload> : IUnitFsmExitableState 
    {
        void Enter(TPayload payload);
    }
}