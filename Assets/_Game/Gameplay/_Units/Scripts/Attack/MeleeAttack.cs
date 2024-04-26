using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class MeleeAttack : UnitAttack
    {
        private float _damage;
        
        public override void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService,
            Transform unitTransform)
        {
            base.Construct(config, faction, audioService, unitTransform);
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