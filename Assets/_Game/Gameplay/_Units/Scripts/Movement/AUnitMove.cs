using _Game.Utils;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Movement
{
    [RequireComponent(typeof(AIPath), typeof(Rigidbody2D))]
    public class AUnitMove : MonoBehaviour, IMovable
    {
        [SerializeField] private AIPath _aiPath;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        private Transform _unitTransform;
        public Vector3 Position => _unitTransform.position;

        private Vector3 _lastPosition;
        private float _updateInterval = 2f; 
        private float _lastUpdateTime = 0;
        
        public bool IsMoving => Vector3.SqrMagnitude(Position - _lastPosition) > 0.00001f;
        
        private Quaternion Rotation
        {
            get => _unitTransform.rotation;
            set => _unitTransform.rotation = value;
        }

        
        public void Construct(Transform unitTransform, float speed)
        {
            _unitTransform = unitTransform;
            _aiPath.maxSpeed = speed;
        }

        public void Move(Vector3 destination)
        {
            if (_aiPath.isStopped)
            {
                _aiPath.isStopped = false;
            }
            
            RotateToTarget(destination);
            
            _aiPath.destination = destination;
            _aiPath.SearchPath();

            if (Time.time - _lastUpdateTime > _updateInterval) {
                _lastPosition = Position;
                _lastUpdateTime = Time.time;
            }
        }
        
        private void RotateToTarget(Vector3 destination)
        {
            if (destination.x < Position.x - Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (destination.x > Position.x + Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 0, 0);
            }
        }


        public void Stop()
        {
            _aiPath.destination = Position;
            _rigidbody2D.velocity = Vector3.zero;
        }

        public void SetSpeedFactor(float speedFactor)
        {
            _aiPath.maxSpeed = speedFactor;
        }
        
        public Vector3 Destination { get; set; }
        public Vector3 DeviationPoint { get; set; }
        public float SpeedFactor => _aiPath.maxSpeed;

        //TODO Delete 
        private void OnDrawGizmos()
        {
            // If the destination vector is set
            if (Destination != Vector3.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(Destination, 0.1f); 
            }
            
            if (DeviationPoint != Vector3.zero && DeviationPoint != Destination)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(DeviationPoint, 0.1f); 
            }
        }
        
        [Button]
        private void ManualInit()
        {
            _aiPath = GetComponent<AIPath>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
            
    }

    public interface IMovable
    {
        Vector3 Position { get; }
        public bool IsMoving { get; }
        void Move(Vector3 destination);
        void Stop();
        void SetSpeedFactor(float speedFactor);
        public Vector3 Destination { set; }
        public Vector3 DeviationPoint { set; }

    }
}
