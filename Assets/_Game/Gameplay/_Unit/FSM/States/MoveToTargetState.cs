using _Game.Gameplay._Unit.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Unit.FSM.States
{
    public class MoveToTargetState : IUnitFsmState
    {
        private readonly UnitFsm _fsm;
        private readonly UnitMove _unitMove;
        private readonly UnitAggroDetection _aggroDetection;
        private readonly UnitAttackDetection _attackDetection;

        public MoveToTargetState(
            UnitFsm fsm,
            UnitMove unitMove,
            UnitAggroDetection aggroDetection,
            UnitAttackDetection attackDetection)
        {
            _unitMove = unitMove;
            _fsm = fsm;
            _aggroDetection = aggroDetection;
            _attackDetection = attackDetection;
        }

        public void Enter()
        {
            //TODO Delete
            Debug.Log("MoveToTargetState ENTERED");
        }

        public void GameUpdate()
        {
            //TODO Delete
            Debug.Log("MoveToTargetState START_UPDATE");
            
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
            Debug.Log("MoveToTargetState END_UPDATE");
        }

        private void UpdateDetectors()
        {
            _aggroDetection.GameUpdate();
            _attackDetection.GameUpdate();
        }

        public void Exit()
        {
            //TODO Delete
            Debug.Log("MoveToTargetState Exited"); 
        }
    }
}