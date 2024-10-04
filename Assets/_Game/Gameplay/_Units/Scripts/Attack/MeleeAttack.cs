using _Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class MeleeAttack : UnitAttack
    {
        private float _damage;
        
        public override void Construct(
            IUnitData unitData,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(unitData, faction, soundService, unitTransform);
            _damage = unitData.Damage;
        }

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            _target?.Damageable.GetDamage(_damage);
            base.OnAttack();
        }
    }
}