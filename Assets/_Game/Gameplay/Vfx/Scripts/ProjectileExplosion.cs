using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class ProjectileExplosion : VfxEntity
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _duration = 0.4f;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private float _age;

        private int _weaponId;

        public void Construct(int weaponId)
        {
            _weaponId = weaponId;
        }

        public void Initialize(Vector3 position)
        {
            Position = position;
            _age = 0;
        }

        public override bool GameUpdate(float deltaTime)
        {
            _age += deltaTime;
            if (_age >= _duration)
            {
                OriginFactory.Reclaim(_weaponId, this);
                return false;
            }
            
            return true;
        }
    }
}