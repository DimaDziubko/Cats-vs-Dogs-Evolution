﻿using _Game.Common;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class VfxSpawner : IVFXProxy
    {
        private readonly IVfxFactory _vfxFactory;

        private readonly GameBehaviourCollection _vfxEntities = new GameBehaviourCollection();
        
        public VfxSpawner(IVfxFactory vfxFactory) => 
            _vfxFactory = vfxFactory;

        public void GameUpdate(float deltaTime) => 
            _vfxEntities.GameUpdate(deltaTime);

        async void IVFXProxy.SpawnMuzzleFlash(MuzzleData data)
        {
            MuzzleFlash muzzle = await _vfxFactory.GetMuzzleFlashAsync(data.Faction, data.WeaponId);
            if(muzzle == null) return;
            muzzle.Initialize(data.Position, data.Direction);
            _vfxEntities.Add(muzzle);
        }

        async void IVFXProxy.SpawnProjectileExplosion(ExplosionData explosionData)
        {
            ProjectileExplosion explosion = await _vfxFactory.GetProjectileExplosionAsync(explosionData.Faction, explosionData.WeaponId);
            if(explosion == null) return;
            explosion.Initialize(explosionData.Positon);
            _vfxEntities.Add(explosion);
        }
        
        void IVFXProxy.SpawnUnitVfx(Vector3 position)
        {
            UnitExplosion explosion =  _vfxFactory.GetUnitExplosion();
            explosion.Initialize(position);
            _vfxEntities.Add(explosion);
            
            //TODO Update and reuse??
            var unitBlot = _vfxFactory.GetUnitBlot();
            unitBlot.Initialize(position);
        }

        public void Cleanup() => 
            _vfxEntities.Clear();

        public void SpawnBasesSmoke(Vector3 basePosition)
        {
            var baseSmoke =  _vfxFactory.GetBaseSmoke();
            baseSmoke.Initialize(basePosition);
            _vfxEntities.Add(baseSmoke);
        }
    }

    public struct ExplosionData
    {
        public Faction Faction;
        public Vector3 Positon;
        public int WeaponId;
    }
}