namespace _Game.Bundles.Units.Common.FSM
{
    public interface IUnitFsmState : IUnitFsmExitableState
    { 
        void Enter();
    }
    
    public interface IUnitFsmExitableState 
    {
        void Exit();
        void GameUpdate();
    }
    
    
    public interface IUnitFsmPayloadedState<TPayload> : IUnitFsmExitableState 
    {
        void Enter(TPayload payload);
    }
}