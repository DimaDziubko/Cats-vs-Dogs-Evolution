using Assets._Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace Assets._Game.Gameplay.Vfx.Scripts
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

        private WeaponType _weaponType;

        public void Construct(WeaponType type)
        {
            _weaponType = type;
        }

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
                OriginFactory.Reclaim(_weaponType, this);
                return false;
            }
            
            return true;
        }
    }
}