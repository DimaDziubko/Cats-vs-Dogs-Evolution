using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.FSM.States;
using Assets._Game.Gameplay._Units.FSM;
using Assets._Game.Gameplay._Units.FSM.States;
using UnityEngine;

namespace _Game.Gameplay._Units.FSM
{
    public class UnitFsm
    {
        private readonly Dictionary<Type, IUnitFsmExitableState> _states = new Dictionary<Type, IUnitFsmExitableState>();
        
        private IUnitFsmExitableState _activeState;

        // public Color StateIndicator()
        // {
        //     if(_activeState is IdleState) return Color.white;
        //     if(_activeState is MoveToPointState) return Color.blue;
        //     if(_activeState is MoveToTargetState) return Color.magenta;
        //     if(_activeState is AttackState) return Color.red;
        //     if(_activeState is DeathState) return Color.yellow;
        //     return Color.black;
        // }
        
        public void AddState(IUnitFsmExitableState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void Enter<TState>() where TState : class, IUnitFsmState
        {
            IUnitFsmState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IUnitFsmPayloadedState<TPayload>
        {
            IUnitFsmPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IUnitFsmExitableState
        {
            _activeState?.Exit();
            
            TState state = GetState<TState>();
            _activeState = state;
            
            return state;
        }

        private TState GetState<TState>() where TState : class, IUnitFsmExitableState =>
            _states[typeof(TState)] as TState;

        public void GameUpdate()
        {
            _activeState?.GameUpdate();
        }

        public void Cleanup()
        {
            foreach (var pair in _states)
            {
                pair.Value.Cleanup();
            }
        }
    }
}