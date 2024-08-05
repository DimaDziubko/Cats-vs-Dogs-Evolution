using _Game.Gameplay._Units.FSM;
using _Game.Gameplay._Units.FSM.States;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Units.Scripts.Movement;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._Units.FSM.States
{
    public class MoveToTargetState : IUnitFsmState
    {
        private readonly UnitFsm _fsm;
        private readonly IMovable _unitMove;
        private readonly UnitAnimator _animator;
        private readonly TargetDetection _aggroDetection;
        private readonly TargetDetection _attackDetection;

        public MoveToTargetState(
            UnitFsm fsm,
            IMovable unitMove,
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

        }

        public void GameUpdate()
        {
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
        }
    }
}