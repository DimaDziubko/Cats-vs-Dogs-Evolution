using _Game.Core.Configs.Models;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class RangeAttack : UnitAttack
    {
        [SerializeField] private Transform _projectileGenerator;
        [SerializeField] private Transform _muzzleTransform;
         
        private int _weaponId;
        private Faction _faction;

        public override void Construct(
            WeaponConfig config,
            Faction faction,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(config, faction, soundService, unitTransform);
            _weaponId = config.Id;
            _faction = faction;
            DisableAttackDelay = 0;
        }

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            var data = new ShootData()
            {
                Faction = _faction,
                Target = _target,
                WeaponId = _weaponId,
                LaunchPosition = _projectileGenerator.position,
                LaunchRotation = _projectileGenerator.rotation,
            };

            if (_muzzleTransform)
            {
                var muzzleData = new MuzzleData()
                {
                    Faction = _faction,
                    WeaponId = _weaponId,
                    Direction = _muzzleTransform.forward,
                    Position = _muzzleTransform.position
                };
                
                _vFXProxy.SpawnMuzzleFlash(muzzleData);  
            }
            
            _shootProxy.Shoot(data);
            
            base.OnAttack();
        }
    }
}