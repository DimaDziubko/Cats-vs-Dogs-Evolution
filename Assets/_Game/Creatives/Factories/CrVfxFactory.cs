using System;
using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core.Factory;
using Assets._Game.Creatives.Creative_1.Scenario;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Creatives.Factories
{
    [CreateAssetMenu(fileName = "CrVfxFactory", menuName = "CrFactories/Vfx")]
    public class CrVfxFactory : GameObjectFactory, IVfxFactory
    {
        [SerializeField] private UnitBlot _blotPrefab;
        [SerializeField] private UnitExplosion _unitExplosionPrefab;
        [FormerlySerializedAs("baseSmokePrefab")] [SerializeField] private BaseSmoke _baseSmokePrefab;

        private readonly Dictionary<VfxType, Queue<VfxEntity>> _sharedPools = new Dictionary<VfxType, Queue<VfxEntity>>();
        
        private readonly Dictionary<int, Queue<MuzzleFlash>> _muzzlesPools =
            new Dictionary<int, Queue<MuzzleFlash>>(6);
        
        private readonly Dictionary<int, Queue<ProjectileExplosion>> _projectileExplosionPools =
            new Dictionary<int, Queue<ProjectileExplosion>>(6);
        
        public UnitBlot GetUnitBlot() => (UnitBlot)Get(VfxType.UnitBlot, _blotPrefab);
        public UnitExplosion GetUnitExplosion() => (UnitExplosion)Get(VfxType.UnitExplosion, _unitExplosionPrefab);
        public BaseSmoke GetBaseSmoke() => (BaseSmoke)Get(VfxType.BaseExplosion, _baseSmokePrefab);
        
        private WeaponData GetWeaponData(Faction faction, int weaponId)
        {
            WeaponData weaponData;
            switch (faction)
            {
                case Faction.Player:
                    weaponData = CrSceneContext.I.ForPlayerWeapon(weaponId);
                    break;
                case Faction.Enemy:
                    weaponData = CrSceneContext.I.ForEnemyWeapon(weaponId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }

            return weaponData;
        }
        
        public ProjectileExplosion GetProjectileExplosion(Faction faction, int weaponId)
        {
            WeaponData weaponData = GetWeaponData(faction, weaponId);
            
            if (!_projectileExplosionPools.TryGetValue(weaponId, out Queue<ProjectileExplosion> pool))
            {
                pool = new Queue<ProjectileExplosion>();
                _projectileExplosionPools[weaponId] = pool;
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
                instance.Construct(weaponId);
                instance.OriginFactory = this;
            }
            
            return instance;
        }


        public MuzzleFlash GetMuzzleFlash(Faction faction, int weaponId)
        {
            WeaponData weaponData = GetWeaponData(faction, weaponId);
            
            if (!_muzzlesPools.TryGetValue(weaponId, out Queue<MuzzleFlash> pool))
            {
                pool = new Queue<MuzzleFlash>();
                _muzzlesPools[weaponId] = pool;
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
                instance.Construct(weaponId);
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
        
        public void Reclaim(int weaponId, MuzzleFlash muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(false);
            if (!_muzzlesPools.TryGetValue(weaponId, out Queue<MuzzleFlash> pool))
            {
                pool = new Queue<MuzzleFlash>();
                _muzzlesPools[weaponId] = pool;
            }
            pool.Enqueue(muzzleFlash);
        }
        
        public void Reclaim(int weaponId, ProjectileExplosion projectileExplosion)
        {
            projectileExplosion.gameObject.SetActive(false);
            if (!_projectileExplosionPools.TryGetValue(weaponId, out Queue<ProjectileExplosion> pool))
            {
                pool = new Queue<ProjectileExplosion>();
                _projectileExplosionPools[weaponId] = pool;
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
