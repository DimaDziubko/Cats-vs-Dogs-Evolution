using _Game.Bundles.Units.Common.Scripts;
using UnityEngine;

namespace _Game.Bundles.Units.Common.FSM.States
{
    public class MoveToTargetState : IUnitFsmState
    {
        private readonly UnitFsm _fsm;
        private readonly UnitMove _unitMove;
        private readonly TargetDetection _aggroDetection;
        private readonly TargetDetection _attackDetection;

        public MoveToTargetState(
            UnitFsm fsm,
            UnitMove unitMove,
            TargetDetection aggroDetection,
            TargetDetection attackDetection)
        {
            _unitMove = unitMove;
            _fsm = fsm;
            _aggroDetection = aggroDetection;
            _attackDetection = attackDetection;
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
                _unitMove.MoveToTarget(_aggroDetection.TargetPosition);
                return;
            }
            
            _fsm.Enter<MoveForwardState>();
            
            //TODO Delete
            //Debug.Log("MoveToTargetState END_UPDATE");
        }

        private void UpdateDetectors()
        {
            _aggroDetection.GameUpdate();
            _attackDetection.GameUpdate();
        }

        public void Exit()
        {
            //TODO Delete
            //Debug.Log("MoveToTargetState Exited"); 
        }
    }
}