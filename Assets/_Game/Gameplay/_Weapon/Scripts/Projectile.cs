using _Game.Core.Configs.Models;
using _Game.Gameplay._Weapon.Factory;
using Assets._Game.Common;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Utils.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    //TODO Fix bug
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Projectile : GameBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private ProjectileMove _move;
        [SerializeField] private Rotator _rotator;
        [SerializeField] private SoundData _soundData;

        private ISoundService _soundService;
        
        [ShowInInspector]
        private ITarget _target;

        private IVFXProxy _vfxProxy;
        protected IInteractionCache _interactionCache;
        public IProjectileFactory OriginFactory { get; set; }

        private bool _isInitialized;


        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        [ShowInInspector]
        public int WeaponId { get; private set; }
        public Faction Faction { get; private set; }

        [ShowInInspector]
        protected float _damage;


        [ShowInInspector]
        protected bool _isDead;


        public virtual void Construct(
            ISoundService soundService,
            Faction faction,
            WeaponConfig config,
            int layer)
        {
            _soundService = soundService;
            
            Faction = faction;
            
            WeaponId = config.Id;
            
            gameObject.layer = layer;
            
            _damage = config.GetProjectileDamageForFaction(faction);
            
            _move.Construct(
                _transform, 
                config.ProjectileSpeed,
                config.TrajectoryWarpFactor);
            
            _isDead = false;
        }

        public override bool GameUpdate()
        {
            if (!_move.IsMoving)
            {
                HandleNotMoving();
            }
            
            if (_isDead)
            {
                Recycle();
                return false;
            }

            if (_isInitialized)
            {
                _move.Move();
            }
            
            if(_rotator) _rotator.Rotate();
            
            return true;
        }

        public void PrepareIntro(
            IVFXProxy vfxProxy,
            Vector3 launchPosition, 
            ITarget target, 
            IInteractionCache cache, 
            Quaternion rotation,
            float speedFactor)
        {
            _vfxProxy = vfxProxy;
            Position = launchPosition;
            Rotation = rotation;
            _target = target;
            _interactionCache = cache;
            
            _move.PrepareIntro(target, launchPosition);
            _move.Reset();
            
            SetSpeedFactor(speedFactor);
            _isInitialized = true;
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        private void OnTriggerEnter2D(Collider2D targetCollider) => 
            HandleCollision(targetCollider);

        protected abstract void HandleCollision(Collider2D targetCollider);

        protected virtual void HandleNotMoving() { }

        protected void SpawnVfx()
        {
            var explosionData = new ExplosionData()
            {
                Faction = Faction,
                Positon = Position,
                WeaponId = WeaponId
            };
            _vfxProxy.SpawnProjectileExplosion(explosionData);
        }
        
        protected void PlaySound()
        {
            if (_soundService != null && _soundData != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_soundData)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }

        public override void SetSpeedFactor(float speedFactor)
        {
            var newSpeed = speedFactor * _move.DefaultSpeed;
            _move.ChangeSpeed(newSpeed);
        }
    }
}