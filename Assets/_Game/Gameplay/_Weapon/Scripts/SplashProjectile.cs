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
        private readonly Collider2D[] _hitBuffer = new Collider2D[5];
        private float _splashRadius;

        private int _collisionMask;
        
        public override void Construct(IAudioService audioService, Faction faction, WeaponConfig config, int layer)
        {
            base.Construct(audioService, faction, config, layer);
            _splashRadius = config.SplashRadius;

            if (layer == Constants.Layer.PLAYER_PROJECTILE)
            {
                _collisionMask = (1 << Constants.Layer.ENEMY) | (1 << Constants.Layer.ENEMY_BASE);
            }
            else if (layer == Constants.Layer.ENEMY_PROJECTILE)
            {
                _collisionMask = (1 << Constants.Layer.PLAYER) | (1 << Constants.Layer.PLAYER_BASE);
            }
            else
            {
                //TODO Delete later
                Debug.LogError("No projectile layer");
                _collisionMask = 0;
            }
        }

        protected override void HandleCollision(Collider2D collider)
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, _splashRadius, _hitBuffer, _collisionMask);
            
            if (count == 0) return;
            
            float[] distances = new float[count];

            for (int i = 0; i < count; i++)
            {
                distances[i] = Vector2.Distance(transform.position, _hitBuffer[i].transform.position);
            }
            
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    if (distances[j] > distances[j + 1])
                    {
                        float tempDist = distances[j];
                        distances[j] = distances[j + 1];
                        distances[j + 1] = tempDist;

                        Collider2D tempCollider = _hitBuffer[j];
                        _hitBuffer[j] = _hitBuffer[j + 1];
                        _hitBuffer[j + 1] = tempCollider;
                    }
                }
            }

            float damageToDeal = _damage;
            
            for (int i = 0; i < count; i++)
            {
                var target = _interactionCache.Get(_hitBuffer[i]);
                if (target?.Damageable != null)
                {
                    target.Damageable.GetDamage(damageToDeal);
                    damageToDeal /= 2; 
                }
            }

            SpawnVfx();
            PlaySound();
            _isDead = true;
        }
    }
}