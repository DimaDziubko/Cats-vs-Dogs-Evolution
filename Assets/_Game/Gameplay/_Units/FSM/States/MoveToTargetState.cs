using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class MoveToTargetState : IUnitFsmState
    {
        private readonly UnitFsm _fsm;
        private readonly UnitMove _unitMove;
        private readonly UnitAnimator _animator;
        private readonly TargetDetection _aggroDetection;
        private readonly TargetDetection _attackDetection;

        public MoveToTargetState(
            UnitFsm fsm,
            UnitMove unitMove,
            TargetDetection aggroDetection,
            TargetDetection attackDetection,
            UnitAnimator animator)
        {
            _unitMove = unitMove;
            _fsm = fsm;
            _aggroDetection = aggroDetection;
            _attackDetection = attackDetection;
            _animator = animator;
        }

        public void Enter()
        {
            //TODO Delete
            //Debug.Log("MoveToTargetState ENTERED");
        }

        public void GameUpdate()
        {
            //TODO Delete
            //Debug.Log("MoveToTargetState START_UPDATE");
            
            UpdateDetectors();
            
            if (_attackDetection.HasTarget)
            {
                _fsm.Enter<AttackState>();
                return;
            }
            
            if (_aggroDetection.HasTarget)
            {
                _unitMove.Move(_aggroDetection.TargetPosition);
                return;
            }
            
            _fsm.Enter<MoveToPointState>();
            
            //TODO Delete
            //Debug.Log("MoveToTargetState END_UPDATE");
        }

        public void Cleanup()
        {
            
        }

        private void UpdateDetectors()
        {
            _aggroDetection.GameUpdate();
            _attackDetection.GameUpdate();
        }

        public void Exit()
        {
            _unitMove.Stop();
            
            //TODO Delete
            //Debug.Log("MoveToTargetState Exited"); 
        }
    }
}