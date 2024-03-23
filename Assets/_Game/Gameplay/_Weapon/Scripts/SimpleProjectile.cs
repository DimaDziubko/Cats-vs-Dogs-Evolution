using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
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
    }
}