using System;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public abstract class ProjectileMove : MonoBehaviour
    {
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
        
        private Transform _targetTransform;
        protected Vector3 TargetPosition => _targetTransform.position;

        protected Vector3 _startPosition;

        protected float _warp;
        protected float _speed;

        public void Construct(Transform projectileTransform, float speed, float warp)
        {
            _transform = projectileTransform;
            _speed = speed;
            _warp = warp;
        }

        public void PrepareIntro(Transform newTarget, Vector3 startPosition)
        {
            _targetTransform = newTarget;
            _startPosition = startPosition;
        }

        public abstract void Reset();

        public abstract void Move();
        
    }
}