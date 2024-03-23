using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class BaseSmoke : VfxEntity
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _duration = 3.0f;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private float _age;

        public void Initialize(Vector3 position)
        {
            Position = position;
            _age = 0;
        }

        public override bool GameUpdate()
        {
            _age += Time.deltaTime;
            if (_age >= _duration)
            {
                OriginFactory.Reclaim( Type, this);
                return false;
            }
            
            return true;
        }
    }
}