﻿using _Game.Common;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class VfxSpawner : IVFXProxy
    {
        private readonly IVfxFactory _vfxFactory;

        private readonly GameBehaviourCollection _vfxEntities = new GameBehaviourCollection();
        
        public VfxSpawner(IVfxFactory vfxFactory)
        {
            _vfxFactory = vfxFactory;
        }

        public void GameUpdate()
        {
            _vfxEntities.GameUpdate();
        }
        
        void IVFXProxy.SpawnMuzzleFlash(MuzzleData data)
        {
            MuzzleFlash muzzle = _vfxFactory.GetMuzzleFlash(data.Faction, data.WeaponType);
            muzzle.Initialize(data.Position, data.Direction);
            _vfxEntities.Add(muzzle);
        }

        void IVFXProxy.SpawnProjectileExplosion(ExplosionData explosionData)
        {
            ProjectileExplosion explosion = _vfxFactory.GetProjectileExplosion(explosionData.Faction, explosionData.Type);
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

        public void Cleanup()
        {
            _vfxEntities.Clear();
        }

        public void SpawnBasesSmoke(Vector3 basePosition)
        {
            var baseSmoke =  _vfxFactory.GetBaseSmoke();
            baseSmoke.Initialize(basePosition);
            _vfxEntities.Add(baseSmoke);
        }
    }

    public class ExplosionData
    {
        public Faction Faction;
        public Vector3 Positon;
        public WeaponType Type;
    }
}