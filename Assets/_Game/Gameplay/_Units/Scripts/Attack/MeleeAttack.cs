using _Game.Core.Configs.Models;
using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace Assets._Game.Gameplay._Units.Scripts.Attack
{
    public class MeleeAttack : UnitAttack
    {
        private float _damage;
        
        public override void Construct(
            WeaponConfig config,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(config, faction, soundService, unitTransform);
            _damage = config.Damage;
        }

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            _target?.Damageable.GetDamage(_damage);
            base.OnAttack();
        }
    }
}