﻿using System;
using System.Collections.Generic;

namespace _Game.UI._MainMenu.State
{
    public class LocalStateMachine
    {
        private readonly Dictionary<Type, ILocalExitableState> _states = new Dictionary<Type, ILocalExitableState>();
        
        private ILocalExitableState _activeState;
        
        public void AddState(ILocalState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void Enter<TState>() where TState : class,ILocalState
        {
            if (_activeState is TState)
            {
                return;
            }
            
            ILocalState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, ILocalPayloadedState<TPayload>
        {
            if (_activeState is TState)
            {
                return;
            }
            
            ILocalPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, ILocalExitableState
        {
            _activeState?.Exit();
            
            TState state = GetState<TState>();
            _activeState = state;
            
            return state;
        }

        private TState GetState<TState>() where TState : class, ILocalExitableState =>
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