using System;
using System.Collections.Generic;
using _Game.Core.DataPresenters.WeaponDataPresenter;
using _Game.Core.Factory;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.Utils;
using Assets._Game.Core.Factory;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using Assets._Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Factory
{
    public enum VfxType
    {
        UnitBlot,
        UnitExplosion,
        BaseExplosion
    }

    [CreateAssetMenu(fileName = "Vfx Factory", menuName = "Factories/Vfx")]
    public class VfxFactory : GameObjectFactory, IVfxFactory
    {
        [SerializeField] private UnitBlot _blotPrefab;
        [SerializeField] private UnitExplosion _unitExplosionPrefab;
        [SerializeField] private BaseSmoke _baseSmokePrefab;
        
        private IWeaponDataPresenter _weaponDataPresenter;

        private readonly Dictionary<VfxType, Queue<VfxEntity>> _sharedPools = 
            new Dictionary<VfxType, Queue<VfxEntity>>();


        private readonly Dictionary<int, Queue<MuzzleFlash>> _muzzlesPools =
            new Dictionary<int, Queue<MuzzleFlash>>(6);

        private readonly Dictionary<int, Queue<ProjectileExplosion>> _projectileExplosionPools =
            new Dictionary<int, Queue<ProjectileExplosion>>(6);

        public UnitBlot GetUnitBlot() => (UnitBlot)Get(VfxType.UnitBlot, _blotPrefab);
        public UnitExplosion GetUnitExplosion() => (UnitExplosion)Get(VfxType.UnitExplosion, _unitExplosionPrefab);
        public BaseSmoke GetBaseSmoke() => (BaseSmoke)Get(VfxType.BaseExplosion, _baseSmokePrefab);
        
        public void Initialize(IWeaponDataPresenter weaponDataPresenter) => 
            _weaponDataPresenter = weaponDataPresenter;

        private WeaponData GetWeaponData(Faction faction, int weaponId)
        {
            WeaponData weaponData;
            switch (faction)
            {
                case Faction.Player:
                    weaponData = _weaponDataPresenter.GetWeaponData(weaponId, Constants.CacheContext.AGE);
                    break;
                case Faction.Enemy:
                    weaponData = _weaponDataPresenter.GetWeaponData(weaponId, Constants.CacheContext.BATTLE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }

            return weaponData;
        }
        
        public ProjectileExplosion GetProjectileExplosion(Faction faction, int weaponId)
        {
            IWeaponData weaponData = GetWeaponData(faction, weaponId);
            if (weaponData == null) return null;
            if (weaponData.ProjectileExplosionPrefab == null) return null;
            
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
        
        public async UniTask<ProjectileExplosion> GetProjectileExplosionAsync(Faction faction, int weaponId)
        {
            IWeaponData weaponData = GetWeaponData(faction, weaponId);
            if (weaponData == null) return null;
            if (weaponData.ProjectileExplosionKey == Constants.ConfigKeys.MISSING_KEY) return null;
            
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
                instance = await CreateGameObjectInstanceAsync<ProjectileExplosion>(weaponData.ProjectileExplosionKey);
                instance.Construct(weaponId);
                instance.OriginFactory = this;
            }
            
            return instance;
        }
        
        public async UniTask<MuzzleFlash> GetMuzzleFlashAsync(Faction faction, int weaponId)
        {
            IWeaponData weaponData = GetWeaponData(faction, weaponId);
            if (weaponData == null ) return null;
            if (weaponData.MuzzleKey == Constants.ConfigKeys.MISSING_KEY) return null;
            
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
                instance = await CreateGameObjectInstanceAsync<MuzzleFlash>(weaponData.MuzzleKey);
                instance.Construct(weaponId);
                instance.OriginFactory = this;
            }
            
            return instance;
        }


        public MuzzleFlash GetMuzzleFlash(Faction faction, int weaponId)
        {
            WeaponData weaponData = GetWeaponData(faction, weaponId);
            if (weaponData == null ) return null;
            if (weaponData.MuzzlePrefab == null) return null;
            
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
            
            base.Cleanup();
        }
    }
}