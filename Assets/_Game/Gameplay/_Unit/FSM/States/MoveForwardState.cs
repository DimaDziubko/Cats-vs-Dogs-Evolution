using _Game.Gameplay._Unit.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Unit.FSM.States
{
    public class MoveForwardState : IUnitFsmPayloadedState<Vector3>, IUnitFsmState
    {
        private Vector3 _direction;
        private readonly UnitMove _unitMove;
        private readonly UnitAggroDetection _unitAggroDetection;
        private readonly UnitAttackDetection _unitAttackDetection;
        private readonly UnitFsm _fsm;

        public MoveForwardState(
            UnitFsm fsm, 
            UnitMove unitMove, 
            UnitAggroDetection unitAggroDetection,
            UnitAttackDetection unitAttackDetection)
        {
            _unitMove = unitMove;
            _unitAggroDetection = unitAggroDetection;
            _unitAttackDetection = unitAttackDetection;
            _fsm = fsm;
        }

        public void Enter(Vector3 direction)
        {
            _direction = direction;
        }

        public void GameUpdate()
        {
            //TODO Delete
            Debug.Log("MoveState START_UPDATE");
            
            UpdateDetectors();
            
            if (_unitAttackDetection.HasTarget)
            {
                _fsm.Enter<AttackState>();
                return;
            }
            if (_unitAggroDetection.HasTarget)
            {
                _fsm.Enter<MoveToTargetState>();
                return;
            }
            
            _unitMove.Move(_direction);
            
            //TODO Delete
            Debug.Log("MoveState END_UPDATE");
        }

        public void Enter()
        {
            //TODO Delete
            Debug.Log("MoveState ENTERED");
        }

        private void UpdateDetectors()
        {
            _unitAttackDetection.GameUpdate();
            _unitAggroDetection.GameUpdate();
        }

        public void Exit()
        {
            //TODO Delete
            Debug.Log("MoveState Exited");
        }
    }
}