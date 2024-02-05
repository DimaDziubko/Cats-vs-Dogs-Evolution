using _Game.Common;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Unit.Factory;
using _Game.Gameplay._Unit.FSM;
using _Game.Gameplay._Unit.FSM.States;
using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    public class Unit : GameBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private UnitHealth _health;
        [SerializeField] private UnitMove _move;
        [SerializeField] private UnitAggroDetection _aggroDetection;
        [SerializeField] private UnitAttackDetection _attackDetection;
        [SerializeField] private UnitAttack _attack;
        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        public Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }


        private Vector3 _movementDirection;

        public Vector3 MovementDirection
        {
            get => _movementDirection;
            set
            {
                _movementDirection = value;
                _fsm.Enter<MoveForwardState, Vector3>(_movementDirection);
            }
        }

        private UnitFsm _fsm;
        private bool _isDead;
        public UnitFactory OriginFactory { get; set; }

        public void Construct(WarriorConfig config)
        {
            InitializeFsm();
            _aggroDetection.Construct();
            _attackDetection.Construct();
            
            //TODO Config
            _attack.Construct(1.1f);
            //TODO Config
            _health.Construct(3);

            _health.Death += OnDeath;
            
            Debug.Log($"Configured unit with health {config.Health}");
        }

        public override bool GameUpdate()
        {
            if (_isDead)
            {
                Recycle();
                return false;
            }
            
            _fsm.GameUpdate();
            return true;
        }

        public override void Recycle()
        {
            _health.Death -= OnDeath;
            
            OriginFactory.Reclaim(this);
        }

        private void InitializeFsm()
        {
            _fsm = new UnitFsm();
            
            IdleState idleState = new IdleState();
            MoveForwardState moveState = new MoveForwardState(_fsm, _move, _aggroDetection, _attackDetection);
            MoveToTargetState moveToTargetState = new MoveToTargetState(_fsm,  _move, _aggroDetection, _attackDetection);
            AttackState attackState = new AttackState(_fsm, _animator, _attack, _aggroDetection, _attackDetection);
            DeathState deathState = new DeathState();

            _fsm.AddState(idleState);
            _fsm.AddState(moveState);
            _fsm.AddState(moveToTargetState);
            _fsm.AddState(attackState);
            _fsm.AddState(deathState);
            
            _fsm.Enter<IdleState>();
        }

        private void OnDeath()
        {
            _isDead = true;
        }
    }
}