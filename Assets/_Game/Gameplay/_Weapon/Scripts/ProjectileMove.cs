using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace Assets._Game.Gameplay._Weapon.Scripts
{
    public abstract class ProjectileMove : MonoBehaviour
    {
        public bool IsMoving { get; protected set; }
        public float DefaultSpeed { get; private set; }

        private Transform _transform;
        protected Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        protected Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }
        
        private ITarget _target;

        protected Vector3 TargetPosition
        {
            get
            {
                if (_target.IsActive)
                {
                    _lastTargetPosition = _target.Transform.position;
                    return _lastTargetPosition;
                }
                
                return _lastTargetPosition;
            }
        }

        protected Vector3 _startPosition;
        private Vector3 _lastTargetPosition;

        protected float _warp;
        protected float _speed;

        public void Construct(Transform projectileTransform, float speed, float warp)
        {
            _transform = projectileTransform;
            _speed = DefaultSpeed = speed;
            _warp = warp;
        }

        public void PrepareIntro(ITarget newTarget, Vector3 startPosition)
        {
            _target = newTarget;
            _startPosition = startPosition;
            IsMoving = true;
        }

        public void ChangeSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }
        
        public abstract void Reset();

        public abstract void Move();
        
    }
}