using _Game.Bundles.Units.Common.Scripts;

namespace _Game.Bundles.Units.Common.FSM.States
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
        }

        public void Enter()
        {
            //TODO Delete
            //Debug.Log("AttackState ENTERED");
            
            _attack.Target = _attackDetection.TargetHealth;
        }


        public void GameUpdate()
        {
            //TODO Delete
            //Debug.Log("AttackState START_UPDATE");
            
            UpdateDetectors();
            
            if (_attackDetection.HasTarget)
            {
                _animator.PlayAttack();
                return;
            }

            if (_aggroDetection.HasTarget)
            {
                _fsm.Enter<MoveToTargetState>();
                return;
            }
            
            _fsm.Enter<MoveForwardState>();
            
            //TODO Delete
            //Debug.Log("AttackState END_UPDATE");
        }

        private void UpdateDetectors()
        {
            _attackDetection.GameUpdate();
            _aggroDetection.GameUpdate();
        }

        public void Exit()
        {
            //TODO Delete
            //Debug.Log("AttackState EXITED");
        }
    }
}