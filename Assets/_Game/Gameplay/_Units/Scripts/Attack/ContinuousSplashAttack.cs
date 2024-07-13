using System;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class ContinuousSplashAttack : UnitAttack
    {
        [SerializeField] private float _splashDamageRatio = 0.2f;
        
        private readonly Collider2D[] _hitBuffer = new Collider2D[3];
        private float _splashRadius;
        private float _damage;
        private int _collisionMask;

        [ShowInInspector, ReadOnly]
        private bool _isAttacking;

        private SoundEmitter _currentSoundEmitter;

        [ShowInInspector, ReadOnly] 
        private string _hash;
        public override void Construct(            
            WeaponConfig config,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(config, faction, soundService, unitTransform);
            _splashRadius = config.SplashRadius;
            _damage = config.Damage;

            switch (faction)
            {
                case Faction.Player:
                    _collisionMask  =  (1 << Constants.Layer.MELEE_ENEMY) 
                                       | (1 << Constants.Layer.ENEMY_BASE) 
                                       | (1 << Constants.Layer.RANGE_ENEMY);
                    break;
                case Faction.Enemy:
                    _collisionMask = (1 << Constants.Layer.MELEE_PLAYER)
                                     | (1 << Constants.Layer.PLAYER_BASE)
                                     | (1 << Constants.Layer.RANGE_PLAYER);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
        
        protected override void OnAttack()
        {
            if (_target == null || !_isActive)
            {
                BreakAttack();
                return;
            }
            
            if (!_isAttacking)
            {
                RotateToTarget(_target.Transform.position);
            
                if (_soundService != null && _soundData != null)
                {
                    _currentSoundEmitter = _soundService.CreateSound()
                        .WithSoundData(_soundData)
                        .WithRandomPitch()
                        .WithPosition(Vector3.zero)
                        .Play();

                    _hash = _currentSoundEmitter.GetHashCode().ToString();
                }
                
                _isAttacking = true;
            }
            
            if(InteractionCache != null)
                ApplyDamage();
        }

        private void ApplyDamage()
        {
            if(_target == null) return;
            int count = Physics2D.OverlapCircleNonAlloc(_target.Transform.position, _splashRadius, _hitBuffer, _collisionMask);
            float[] distances = new float[count];
            
            for (int i = 0; i < count; i++)
            {
                distances[i] = Vector2.Distance(_target.Transform.position, _hitBuffer[i].transform.position);
            }

            Array.Sort(distances, _hitBuffer, 0, count);

            float damageToDeal = _damage;
            
            for (int i = 0; i < count; i++)
            {
                var target = InteractionCache.Get(_hitBuffer[i]);
                if (target?.Damageable != null)
                {
                    target.Damageable.GetDamage(damageToDeal);
                    damageToDeal = _splashDamageRatio * _damage; 
                }
            }
        }

        public override void SetPaused(in bool isPaused)
        {
            if(_currentSoundEmitter != null)
                _currentSoundEmitter.SetPaused(isPaused);
        }

        public void BreakAttack()
        {
            if (_currentSoundEmitter != null && 
                _currentSoundEmitter.IsPlaying)
            {
                _currentSoundEmitter.Stop();
                _currentSoundEmitter = null;
            }
            
            _isAttacking = false;
        }
    }
}