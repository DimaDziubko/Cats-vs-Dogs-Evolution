using Assets._Game.Core.Services.Random;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts.Movement;
using UnityEngine;

namespace Assets._Game.Gameplay._Units.FSM.States
{
    public class MoveToPointState : IUnitFsmPayloadedState<Vector3>, IUnitFsmState
    {
        private const float MAX_PATH_DEVIATION = 0.5f;
        
        private const float DECISION_TIME_MIN = 5;
        private const float DECISION_TIME_MAX = 10;
        
        private const float MIN_DISTANCE_TO_DEVIATION_POINT = 0.5f;
        
        private const float NOISE_FREQUENCY = 1f;

        private readonly IRandomService _random;
        private readonly IMovable _unitMove;
        private readonly UnitAnimator _animator;
        private readonly TargetDetection _unitAggroDetection;
        private readonly TargetDetection _unitAttackDetection;
        private readonly UnitFsm _fsm;

        private Vector3 _destination;
        private float _lastDecisionTime;
        
        private Vector3 _currentTarget;
        private float _lastPathUpdateTime;

        private float _pathUpdateFrequency;
        
        public MoveToPointState(
            UnitFsm fsm, 
            IMovable unitMove, 
            TargetDetection unitAggroDetection,
            TargetDetection unitAttackDetection,
            UnitAnimator animator,
            IRandomService random)
        {
            _unitMove = unitMove;
            _unitAggroDetection = unitAggroDetection;
            _unitAttackDetection = unitAttackDetection;
            _fsm = fsm;
            _animator = animator;
            _random = random;
        }

        public void Enter(Vector3 destination)
        {
            _destination = destination;
            CalculateRandomPathUpdateFrequency();
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
            
            if (NeedNewPath())
            {
                UpdatePath();
            }

            _unitMove.Move(_currentTarget);
            
        }

        public void Cleanup() { }
        public void Enter() { }

        public void Exit()
        {
            _unitMove.Stop();
        }

        private void CalculateRandomPathUpdateFrequency()
        {
            _pathUpdateFrequency = _random.Next(DECISION_TIME_MIN, DECISION_TIME_MAX);
        }

        private bool NeedNewPath()
        {
            return Time.time - _lastPathUpdateTime >= _pathUpdateFrequency
                   || Vector3.Distance(_unitMove.Position, _currentTarget) <= MIN_DISTANCE_TO_DEVIATION_POINT
                   || !_unitMove.IsMoving;
        }

        private void UpdatePath()
        {
            _currentTarget = GetRandomizedPath(_unitMove.Position, _destination, NOISE_FREQUENCY, MAX_PATH_DEVIATION);
            _lastPathUpdateTime = Time.time;
            
            CalculateRandomPathUpdateFrequency();
            
            //For GUI
            // _unitMove.Destination = _destination;
            // _unitMove.DeviationPoint = _currentTarget;
        }


        private Vector3 GetRandomizedPath(Vector3 start, Vector3 end, float noiseFrequency, float maxDeviation)
        {
            float deviationScale = 2.0f;
            float noiseOffset = 1.0f; 
            float pathMidpoint = 0.5f; 

            float noiseValue = Mathf.PerlinNoise(Time.time * noiseFrequency, 0.0f) * deviationScale - noiseOffset; 
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward); 
            Vector3 deviation = perpendicular * noiseValue * maxDeviation; 

            Vector3 targetPoint = Vector3.Lerp(start, end, pathMidpoint) + deviation;
            return targetPoint;
        }

        private void UpdateDetectors()
        {
            _unitAttackDetection.GameUpdate();
            _unitAggroDetection.GameUpdate();
        }
    }
}