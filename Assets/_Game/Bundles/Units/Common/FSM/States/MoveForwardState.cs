using _Game.Bundles.Units.Common.Scripts;
using UnityEngine;

namespace _Game.Bundles.Units.Common.FSM.States
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
            
        }

        public void Enter()
        {

        }

        private void UpdateDetectors()
        {
            _unitAttackDetection.GameUpdate();
            _unitAggroDetection.GameUpdate();
        }

        public void Exit()
        {

        }
    }
}