using _Game.Common;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Projectile : GameBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private ProjectileMove _move;

        [SerializeField] private AudioClip _hitSFX;
        
        private IAudioService _audioService;
        
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

        public WeaponType Type { get; private set; }
        public Faction Faction { get; private set; }

        protected float _damage;

        protected bool _isDead;


        public virtual void Construct(
            IAudioService audioService,
            Faction faction,
            WeaponConfig config,
            int layer)
        {
            _audioService = audioService;
            
            Faction = faction;
            
            Type = config.WeaponType;
            
            gameObject.layer = layer;
            
            _damage = config.Damage;
            _move.Construct(
                _transform, 
                config.ProjectileSpeed,
                config.TrajectoryWarpFactor);
            _isDead = false;
        }

        public override bool GameUpdate()
        {
            if (_isDead || !_target.IsActive)
            {
                Recycle();
                return false;
            }


            if (_isInitialized && _target.Transform != null)
            {
                _move.Move();
            }
            
            return true;
        }

        public void PrepareIntro(
            IVFXProxy vfxProxy,
            Vector3 launchPosition, 
            ITarget target, 
            IInteractionCache cache, 
            Quaternion rotation)
        {
            _vfxProxy = vfxProxy;
            Position = launchPosition;
            Rotation = rotation;
            _target = target;
            _interactionCache = cache;
            
            _move.PrepareIntro(target.Transform, launchPosition);
            _move.Reset();
            
            _isInitialized = true;
        }

        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        private void OnTriggerEnter2D(Collider2D targetCollider)
        {
            HandleCollision(targetCollider);
        }

        protected abstract void HandleCollision(Collider2D targetCollider);

        protected void SpawnVfx()
        {
            var explosionData = new ExplosionData()
            {
                Faction = Faction,
                Positon = Position,
                Type = Type
            };
            _vfxProxy.SpawnProjectileExplosion(explosionData);
        }
        
        protected void PlaySound()
        {
            if (_audioService != null && _hitSFX != null)
            {
                _audioService.PlayOneShot(_hitSFX);
            }
        }
    }
}