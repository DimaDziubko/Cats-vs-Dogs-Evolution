using System;
using _Game.Common.Animation.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
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
        private const string ATTACK_PER_SECOND = "DPS";

        private readonly int _idleStateHash = Animator.StringToHash(IDLE_ANIMATOR_STATE);
        private readonly int _attackStateHash = Animator.StringToHash(ATTACK_ANIMATOR_STATE);
        private readonly int _dPSHash = Animator.StringToHash(ATTACK_PER_SECOND);

        [SerializeField] private Animator _animator;

        [SerializeField] private Transform _weaponAimBone;

        [SerializeField] private float _minAngle = -60f;
        [SerializeField] private float _maxAngle = 60f;
        public float Speed => _animator.speed;
        public float DefaultSpeed { get; private set; }

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;
        
        [ShowInInspector, ReadOnly]
        public AnimatorState State { get; private set; }

        [ShowInInspector, ReadOnly] 
        private Transform _target;

        private LookAtJob _lookAtJob;
        private AnimationScriptPlayable _lookAtPlayable;
        
        [ShowInInspector, ReadOnly] 
        private bool IsIkActive => _lookAtJob.isActive;

        private bool _isLookAtJobInitialized;
        private bool _lookAtPlayableConnected;

        private float _attackPerSecond;

        [ShowInInspector, ReadOnly] 
        public bool IsAttacking => State == AnimatorState.Attack;
        
        public void PlayAttack()
        {
            _animator.SetFloat(_dPSHash, _attackPerSecond);
            _animator.SetBool(_attackStateHash, true);
        }

        public void StopAttack() => _animator.SetBool(_attackStateHash, false);
        public void ResetToIdle() => _animator.SetTrigger(_idleStateHash);

        public void Construct(float attackPerSecond)
        {
            InitializeLookAtJob();
            _attackPerSecond = attackPerSecond;
            DefaultSpeed = _animator.speed;
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
            _target = targetTransform;
            if(!_weaponAimBone) return;
            UpdateLookAtJobTarget();
        }
        
        private void UpdateLookAtJobTarget()
        {
            _lookAtJob.target = _animator.BindSceneTransform(_target);
            _lookAtPlayable.SetJobData(_lookAtJob);
        }

        public void SetSpeed(float speed)
        {
            _animator.speed = speed;
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