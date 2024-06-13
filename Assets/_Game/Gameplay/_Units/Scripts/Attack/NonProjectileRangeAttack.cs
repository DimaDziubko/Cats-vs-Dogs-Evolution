﻿using _Game.Core.Configs.Models;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class NonProjectileRangeAttack : UnitAttack
    {
        [SerializeField] private Transform _muzzleTransform;
        
        private WeaponType _type;
        private Faction _faction;
        
        private WeaponConfig _config;

        public override void Construct(
            WeaponConfig config,
            Faction faction,
            IAudioService audioService,
            ISoundService soundService,
            Transform unitTransform)
        {
            base.Construct(config, faction, audioService, soundService, unitTransform);
            
            _type = config.WeaponType;
            _faction = faction;
            _config = config;
        }

        protected override void OnAttack()
        {
            if(_target == null || !_isActive) return;
            
            var muzzleData = new MuzzleData()
            {
                Faction = _faction,
                WeaponType = _type,
                Direction = _muzzleTransform.forward,
                Position = _muzzleTransform.position
            };
            
            _vFXProxy.SpawnMuzzleFlash(muzzleData);
            
            _target?.Damageable.GetDamage(_config.Damage);
            base.OnAttack();
        }
    }
}