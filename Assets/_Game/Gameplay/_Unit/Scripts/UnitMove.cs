using UnityEngine;

namespace _Game.Gameplay._Unit.Scripts
{
    public class UnitMove : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        //TODO Config
        private float _speed = 0.25f;
        
        public void Construct()
        {
            
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