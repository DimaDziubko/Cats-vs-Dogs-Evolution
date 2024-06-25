using System;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class SplashProjectile : Projectile
    {
        [SerializeField] private float _splashDamageRatio = 0.2f;
        
        private readonly Collider2D[] _hitBuffer = new Collider2D[3];
        private float _splashRadius;

        private int _collisionMask;
        
        public override void Construct(ISoundService soundService, Faction faction, WeaponConfig config, int layer)
        {
            base.Construct(soundService, faction, config, layer);
            _splashRadius = config.SplashRadius;

            if (layer == Constants.Layer.PLAYER_PROJECTILE)
            {
                _collisionMask = (1 << Constants.Layer.MELEE_ENEMY) | (1 << Constants.Layer.ENEMY_BASE) | (1 << Constants.Layer.RANGE_ENEMY);
            }
            else if (layer == Constants.Layer.ENEMY_PROJECTILE)
            {
                _collisionMask = (1 << Constants.Layer.MELEE_PLAYER) | (1 << Constants.Layer.PLAYER_BASE) | (1 << Constants.Layer.RANGE_PLAYER);
            }
            else
            {
                _collisionMask = 0;
            }
        }

        protected override void HandleCollision(Collider2D collider)
        {
            if (collider != null)
            {
                ApplyDamageAndEffects(); 
                //Debug.Log($"Handle collision triggered with collider inside _isDead {_isDead}");
            }
        }
        
        protected override void HandleNotMoving()
        {
            //Debug.Log("Handle not moving because projectile stopped.");
            ApplyDamageAndEffects();
        }
        
        // private void ApplyDamageAndEffects()
        // {
        //     int count = Physics2D.OverlapCircleNonAlloc(transform.position, _splashRadius, _hitBuffer, _collisionMask);
        //
        //     if (count == 0) 
        //     {
        //         _isDead = true;
        //         SpawnVfx();
        //         PlaySound();
        //         return;
        //     }
        //
        //     float[] distances = new float[count];
        //     for (int i = 0; i < count; i++)
        //     {
        //         distances[i] = Vector2.Distance(transform.position, _hitBuffer[i].transform.position);
        //     }
        //
        //     Array.Sort(distances, _hitBuffer, 0, count);
        //
        //     float damageToDeal = _damage;
        //     for (int i = 0; i < count; i++)
        //     {
        //         var target = _interactionCache.Get(_hitBuffer[i]);
        //         if (target?.Damageable != null)
        //         {
        //             target.Damageable.GetDamage(damageToDeal);
        //             damageToDeal /= 2; 
        //         }
        //     }
        //
        //     SpawnVfx();
        //     PlaySound();
        //     _isDead = true;
        // }
        
        private void ApplyDamageAndEffects()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, _splashRadius, _hitBuffer, _collisionMask);
    
            if (count == 0) 
            {
                _isDead = true;
                SpawnVfx();
                PlaySound();
                return;
            }

            float[] distances = new float[count];
            
            for (int i = 0; i < count; i++)
            {
                distances[i] = Vector2.Distance(transform.position, _hitBuffer[i].transform.position);
            }

            Array.Sort(distances, _hitBuffer, 0, count);

            float damageToDeal = _damage;
            
            for (int i = 0; i < count; i++)
            {
                var target = _interactionCache.Get(_hitBuffer[i]);
                if (target?.Damageable != null)
                {
                    target.Damageable.GetDamage(damageToDeal);
                    damageToDeal = _splashDamageRatio * _damage; 
                }
            }

            SpawnVfx();
            PlaySound();
            _isDead = true;
        }
    }
}