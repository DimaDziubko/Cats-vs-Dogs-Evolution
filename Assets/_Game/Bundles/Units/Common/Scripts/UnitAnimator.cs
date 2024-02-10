using System;
using _Game.Common.Animation.Scripts;
using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class UnitAnimator : MonoBehaviour, IAnimationStateReader
    {
        private const string IDLE_ANIMATOR_STATE = "Idle";
        private const string ATTACK_ANIMATOR_STATE = "Attack";

        private readonly int _idleStateHash = Animator.StringToHash(IDLE_ANIMATOR_STATE);
        private readonly int _attackStateHash = Animator.StringToHash(ATTACK_ANIMATOR_STATE);
        
        [SerializeField] private Animator _animator;

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public AnimatorState State { get; private set; }

        public bool IsAttacking => State == AnimatorState.Attack;
        
        public void PlayAttack() => _animator.SetTrigger(_attackStateHash);
        public void ResetToIdle() => _animator.SetTrigger(_idleStateHash);

        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash)
        {
            StateExited?.Invoke(StateFor(stateHash));
        }

        private AnimatorState StateFor(int stateHash)
        {
            AnimatorState state;
            if (stateHash == _idleStateHash)
                state = AnimatorState.Idle;
            else if (stateHash == _attackStateHash)
                state = AnimatorState.Attack;
            else
                state = AnimatorState.Unknown;
            return state;
        }
    }
}