using System;
using System.Collections.Generic;

namespace _Game.UI._MainMenu.State
{
    public class MenuStateMachine
    {
        private readonly Dictionary<Type, IMenuExitableState> _states = new Dictionary<Type, IMenuExitableState>();
        
        private IMenuExitableState _activeState;
        
        public void AddState(IMenuState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void Enter<TState>() where TState : class,IMenuState
        {
            if (_activeState is TState)
            {
                return;
            }
            
            IMenuState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IMenuPayloadedState<TPayload>
        {
            if (_activeState is TState)
            {
                return;
            }
            
            IMenuPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IMenuExitableState
        {
            _activeState?.Exit();
            
            TState state = GetState<TState>();
            _activeState = state;
            
            return state;
        }

        private TState GetState<TState>() where TState : class, IMenuExitableState =>
            _states[typeof(TState)] as TState;
        

        public void Cleanup()
        {
            foreach (var pair in _states)
            {
                pair.Value.Cleanup();
            }
        }
    }
}