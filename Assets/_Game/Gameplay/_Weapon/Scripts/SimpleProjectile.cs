using UnityEngine;

namespace Assets._Game.Gameplay._Weapon.Scripts
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class SimpleProjectile : Projectile
    {
        protected override void HandleCollision(Collider2D collider)
        {
            var target = _interactionCache.Get(collider);
            target?.Damageable.GetDamage(_damage);
            PlaySound();
            _isDead = true;
        }

        protected override void HandleNotMoving()
        {
            _isDead = true;
        }
    }
}