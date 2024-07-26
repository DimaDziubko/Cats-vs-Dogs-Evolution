using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.FSM;
using Assets._Game.Gameplay._Units.FSM.States;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class AttackState : IUnitFsmState
    {
        private readonly UnitFsm _fsm;
        private readonly UnitAttack _attack;
        private readonly UnitAnimator _animator;
        private readonly TargetDetection _aggroDetection;
        private readonly TargetDetection _attackDetection;

        public AttackState(
            UnitFsm fsm, 
            UnitAnimator animator, 
            UnitAttack attack,
            TargetDetection aggroDetection,
            TargetDetection attackDetection)
        {
            _animator = animator;
            _attack = attack;
            _fsm = fsm;
            _aggroDetection = aggroDetection;
            _attackDetection = attackDetection;

            _attackDetection.TargetUpdated += OnUpdatedTarget;
        }

        private void OnUpdatedTarget(ITarget target)
        {
            if(target == null) return;

            _attack.SetTarget(target);
            _animator.SetTarget(target.Transform);
        }

        public void Enter()
        {
            _animator.PlayAttack();
        }


        public void GameUpdate()
        {
            UpdateDetectors();

            if (_attackDetection.HasTarget)
            {
                return;
            }

            if (_aggroDetection.HasTarget)
            {
                _fsm.Enter<MoveToTargetState>();
                return;
            }
            
            _fsm.Enter<MoveToPointState>();
        }

        public void Cleanup()
        {
            _attackDetection.TargetUpdated -= OnUpdatedTarget;
        }

        private void UpdateDetectors()
        {
            _attackDetection.GameUpdate();
            _aggroDetection.GameUpdate();
        }

        public void Exit()
        {
            _animator.StopAttack();
        }
    }
}