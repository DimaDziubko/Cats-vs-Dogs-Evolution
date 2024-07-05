using System;
using System.Collections.Generic;
using Assets._Game.Core.Factory;
using Assets._Game.Creatives.Creative_1.Scenario;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Factory;
using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets._Game.Creatives.Factories
{
    [CreateAssetMenu(fileName = "CrVfxFactory", menuName = "CrFactories/Vfx")]
    public class CrVfxFactory : GameObjectFactory, IVfxFactory
    {
        [SerializeField] private UnitBlot _blotPrefab;
        [SerializeField] private UnitExplosion _unitExplosionPrefab;
        [FormerlySerializedAs("baseSmokePrefab")] [SerializeField] private BaseSmoke _baseSmokePrefab;

        private readonly Dictionary<VfxType, Queue<VfxEntity>> _sharedPools = new Dictionary<VfxType, Queue<VfxEntity>>();
        
        private readonly Dictionary<WeaponType, Queue<MuzzleFlash>> _muzzlesPools =
            new Dictionary<WeaponType, Queue<MuzzleFlash>>(6);
        
        private readonly Dictionary<WeaponType, Queue<ProjectileExplosion>> _projectileExplosionPools =
            new Dictionary<WeaponType, Queue<ProjectileExplosion>>(6);
        
        public UnitBlot GetUnitBlot() => (UnitBlot)Get(VfxType.UnitBlot, _blotPrefab);
        public UnitExplosion GetUnitExplosion() => (UnitExplosion)Get(VfxType.UnitExplosion, _unitExplosionPrefab);
        public BaseSmoke GetBaseSmoke() => (BaseSmoke)Get(VfxType.BaseExplosion, _baseSmokePrefab);
        
        private WeaponData GetWeaponData(Faction faction, WeaponType type)
        {
            WeaponData weaponData;
            switch (faction)
            {
                case Faction.Player:
                    weaponData = CrSceneContext.I.ForPlayerWeapon(type);
                    break;
                case Faction.Enemy:
                    weaponData = CrSceneContext.I.ForEnemyWeapon(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }

            return weaponData;
        }
        
        public ProjectileExplosion GetProjectileExplosion(Faction faction, WeaponType type)
        {
            WeaponData weaponData = GetWeaponData(faction, type);
            
            if (!_projectileExplosionPools.TryGetValue(type, out Queue<ProjectileExplosion> pool))
            {
                pool = new Queue<ProjectileExplosion>();
                _projectileExplosionPools[type] = pool;
            }
            
            ProjectileExplosion instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = CreateGameObjectInstance(weaponData.ProjectileExplosionPrefab);
                instance.Construct(type);
                instance.OriginFactory = this;
            }
            
            return instance;
        }


        public MuzzleFlash GetMuzzleFlash(Faction faction, WeaponType type)
        {
            WeaponData weaponData = GetWeaponData(faction, type);
            
            if (!_muzzlesPools.TryGetValue(type, out Queue<MuzzleFlash> pool))
            {
                pool = new Queue<MuzzleFlash>();
                _muzzlesPools[type] = pool;
            }
            
            MuzzleFlash instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = CreateGameObjectInstance(weaponData.MuzzlePrefab);
                instance.Construct(type);
                instance.OriginFactory = this;
            }
            
            return instance;
        }


        private VfxEntity Get(VfxType type, VfxEntity prefab)
        {
            if (!_sharedPools.TryGetValue(type, out Queue<VfxEntity> pool))
            {
                pool = new Queue<VfxEntity>();
                _sharedPools[type] = pool;
            }

            VfxEntity instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = CreateGameObjectInstance(prefab);
                instance.OriginFactory = this;
                instance.Type = type;
            }

            return instance;
        }

        public void Reclaim(VfxType type, VfxEntity entity)
        {
            entity.gameObject.SetActive(false);
            if (!_sharedPools.TryGetValue(type, out Queue<VfxEntity> pool))
            {
                pool = new Queue<VfxEntity>();
                _sharedPools[type] = pool;
            }
            pool.Enqueue(entity);
        }
        
        public void Reclaim(WeaponType type, MuzzleFlash muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(false);
            if (!_muzzlesPools.TryGetValue(type, out Queue<MuzzleFlash> pool))
            {
                pool = new Queue<MuzzleFlash>();
                _muzzlesPools[type] = pool;
            }
            pool.Enqueue(muzzleFlash);
        }
        
        public void Reclaim(WeaponType type, ProjectileExplosion projectileExplosion)
        {
            projectileExplosion.gameObject.SetActive(false);
            if (!_projectileExplosionPools.TryGetValue(type, out Queue<ProjectileExplosion> pool))
            {
                pool = new Queue<ProjectileExplosion>();
                _projectileExplosionPools[type] = pool;
            }
            pool.Enqueue(projectileExplosion);
        }

        public override void Cleanup()
        {
            foreach (var pool in _sharedPools.Values)
            {
                while (pool.Count > 0)
                {
                    VfxEntity entity = pool.Dequeue();
                    Destroy(entity.gameObject);
                }
            }
            _sharedPools.Clear();
            
            foreach (var pool in _muzzlesPools.Values)
            {
                while (pool.Count > 0)
                {
                    MuzzleFlash muzzleFlash = pool.Dequeue();
                    Destroy(muzzleFlash.gameObject);
                }
            }
            _muzzlesPools.Clear();
            
            foreach (var pool in  _projectileExplosionPools.Values)
            {
                while (pool.Count > 0)
                {
                    ProjectileExplosion projectileExplosion = pool.Dequeue();
                    Destroy(projectileExplosion.gameObject);
                }
            }
            _projectileExplosionPools.Clear();
        }
    }
}
