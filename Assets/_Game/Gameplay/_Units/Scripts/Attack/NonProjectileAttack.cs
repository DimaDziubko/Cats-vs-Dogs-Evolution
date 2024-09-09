using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class NonProjectileAttack : UnitAttack
    {
        [SerializeField] private Transform _muzzleTransform;
        
        private int _weaponId;
        private Faction _faction;
        
        private float _damage;

        public override void Construct(
            IUnitData unitData,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(unitData, faction, soundService, unitTransform);
            
            _weaponId = unitData.WeaponId;
            _faction = faction;
            _damage = unitData.Damage;
        }

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            var muzzleData = new MuzzleData()
            {
                Faction = _faction,
                WeaponId = _weaponId,
                Direction = _muzzleTransform.forward,
                Position = _muzzleTransform.position
            };
            
            _vFXProxy.SpawnMuzzleFlash(muzzleData);
            
            _target?.Damageable.GetDamage(_damage);
            base.OnAttack();
        }
    }
}