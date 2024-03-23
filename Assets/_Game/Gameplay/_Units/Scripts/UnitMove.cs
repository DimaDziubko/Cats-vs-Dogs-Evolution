using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Gameplay._Units.Scripts
{
    public class UnitMove : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        
        private Transform _transform;
        public Vector3 Position => _transform.position;

        private float _speed;
        
        public void Construct(Transform transform, float speed)
        {
            _transform = transform;
            _agent.speed = speed;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.acceleration = 8;
        }
        
        public void Move(Vector3 destination)
        {
            if (_agent.isStopped)
            {
                _agent.isStopped = false;
            }
            
            _agent.SetDestination(destination);
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }

        public Vector3 Destination { get; set; }
        public Vector3 DeviationPoint { get; set; }
        public float SpeedFactor => _agent.speed;

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
        public void SetSpeedFactor(float speedFactor)
        {
            _agent.speed = speedFactor;
        }

        //Helper

        [ExecuteAlways]
        [Button]
        private void ManualInit()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }
}