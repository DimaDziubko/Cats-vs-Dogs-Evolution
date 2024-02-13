using UnityEngine;

namespace _Game.Bundles.Units.Common.Scripts
{
    public class UnitMove : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private float _speed;
        
        public void Construct(float speed)
        {
            _speed = speed;
        }
        
        public void Move(Vector3 direction)
        {
            Position += _speed * direction * Time.deltaTime;
        }

        public void MoveToTarget(Vector3 destination)
        {
            Vector3 direction = (destination - Position).normalized;
            Move(direction);
        }
        
    }
}