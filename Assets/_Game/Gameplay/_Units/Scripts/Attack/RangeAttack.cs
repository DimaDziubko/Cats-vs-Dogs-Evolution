using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class RangeAttack : UnitAttack
    {
        [SerializeField] private Transform _projectileGenerator;
        [SerializeField] private Transform _muzzleTransform;
         
        private WeaponType _type;
        private Faction _faction;

        public override void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService,
            Transform unitTransform)
        {
            base.Construct(config, faction, audioService, unitTransform);
            _type = config.WeaponType;
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
                WeaponType = _type,
                LaunchPosition = _projectileGenerator.position,
                LaunchRotation = _projectileGenerator.rotation,
            };

            if (_muzzleTransform)
            {
                var muzzleData = new MuzzleData()
                {
                    Faction = _faction,
                    WeaponType = _type,
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