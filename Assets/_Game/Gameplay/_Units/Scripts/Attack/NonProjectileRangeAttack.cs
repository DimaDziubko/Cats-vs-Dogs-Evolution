using _Game.Core.Configs.Models;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class NonProjectileRangeAttack : UnitAttack
    {
        [SerializeField] private Transform _muzzleTransform;
        
        private int _weaponId;
        private Faction _faction;
        
        private WeaponConfig _config;

        public override void Construct(
            WeaponConfig config,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(config, faction, soundService, unitTransform);
            
            _weaponId = config.Id;
            _faction = faction;
            _config = config;
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
            
            _target?.Damageable.GetDamage(_config.Damage);
            base.OnAttack();
        }
    }
}