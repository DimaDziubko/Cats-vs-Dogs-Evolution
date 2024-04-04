using System;
using _Game.Common.Animation.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace _Game.Gameplay._Units.Scripts
{
    public class UnitAnimator : MonoBehaviour, IAnimationStateReader
    {
        private const string IDLE_ANIMATOR_STATE = "Idle";
        private const string ATTACK_ANIMATOR_STATE = "Attack";
        
        private readonly int _idleStateHash = Animator.StringToHash(IDLE_ANIMATOR_STATE);
        private readonly int _attackStateHash = Animator.StringToHash(ATTACK_ANIMATOR_STATE);
        
        [SerializeField] private Animator _animator;

        [SerializeField] private Transform _weaponAimBone;

        [SerializeField] private float _minAngle = -60f;
        [SerializeField] private float _maxAngle = 60f;

        private Transform _target;
        
        private LookAtJob _lookAtJob;
        private AnimationScriptPlayable _lookAtPlayable;

        [ShowInInspector] 
        private bool IsIkActive => _lookAtJob.isActive;
        
        private bool _isLookAtJobInitialized;
        private bool _lookAtPlayableConnected;
        
        public float SpeedFactor => _animator.speed;
        
        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public AnimatorState State { get; private set; }

        public bool IsAttacking => State == AnimatorState.Attack;
        
        public void PlayAttack() => _animator.SetBool(_attackStateHash, true);
        public void StopAttack() => _animator.SetBool(_attackStateHash, false);
        public void ResetToIdle() => _animator.SetTrigger(_idleStateHash);

        public void Construct()
        {
            InitializeLookAtJob();
        }
        
        private void InitializeLookAtJob()
        {
            var graph = _animator.playableGraph;
            if(!_weaponAimBone) return;
            _lookAtJob = new LookAtJob
            {
                joint = _animator.BindStreamTransform(_weaponAimBone),
                axis = Vector3.right,
                minAngle = _minAngle,
                maxAngle = _maxAngle,
                isActive = false,
            };
            
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "output", _animator);

            _lookAtPlayable = AnimationScriptPlayable.Create(graph, _lookAtJob);

            output.SetSourcePlayable(_lookAtPlayable);
        }
        
        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash)
        {
            StateExited?.Invoke(StateFor(stateHash));
        }

        public void SetTarget(Transform targetTransform)
        {
            if(!_weaponAimBone) return;
            _target = targetTransform;
            UpdateLookAtJobTarget();
        }
        
        private void UpdateLookAtJobTarget()
        {
            _lookAtJob.target = _animator.BindSceneTransform(_target);
            _lookAtPlayable.SetJobData(_lookAtJob);
        }

        public void SetSpeedFactor(float speedFactor)
        {
            _animator.speed = speedFactor;
        }

        //Animation event
        public void ActivateAiming()
        {
            if(!_weaponAimBone) return;
            if(_lookAtJob.isActive) return;
            _lookAtJob.isActive = true;
            _lookAtPlayable.SetJobData(_lookAtJob);
        }

        //Animation event
        public void DeactivateAiming()
        {
            if(!_weaponAimBone) return;
            if(!_lookAtJob.isActive) return;
            _lookAtJob.isActive = false;
            _lookAtPlayable.SetJobData(_lookAtJob);
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