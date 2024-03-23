using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class MeleeAttack : UnitAttack
    {
        private float _damage;
        
        public override void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService)
        {
            base.Construct(config, faction, audioService);
            _damage = config.Damage;
        }

        protected override void OnAttack()
        {
            _target?.Damageable.GetDamage(_damage);
            base.OnAttack();
        }
    }
}