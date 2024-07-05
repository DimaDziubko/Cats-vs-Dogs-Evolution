using System;
using System.Collections.Generic;
using Assets._Game.Core.DataPresenters.WeaponDataPresenter;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Utils;
using UnityEngine;

namespace Assets._Game.Gameplay._Weapon.Factory
{
    public interface IProjectileFactory
    {
        Projectile Get(Faction faction, WeaponType type);
        public void Reclaim(Projectile proj);
    }

    [CreateAssetMenu(fileName = "Projectile Factory", menuName = "Factories/Projectile")]
    public class ProjectileFactory : GameObjectFactory, IProjectileFactory
    {
        private IWeaponDataPresenter _weaponDataPresenter;
        private ISoundService _soundService;


        private readonly Dictionary<(Faction, WeaponType), Queue<Projectile>> _projectilesPools =
            new Dictionary<(Faction, WeaponType), Queue<Projectile>>();

        public void Initialize(
            ISoundService soundService,
            IWeaponDataPresenter weaponDataPresenter)
        {
            _weaponDataPresenter = weaponDataPresenter;
            _soundService = soundService;
        }
        
        public Projectile Get(Faction faction,  WeaponType type)
        {
            WeaponData weaponData = GetWeaponData(faction, type);
            
            if (!_projectilesPools.TryGetValue((faction, type), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(faction, type)] = pool;
            }
            
            Projectile instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = CreateGameObjectInstance(weaponData.ProjectilePrefab);
                instance.OriginFactory = this;
            }

            instance.Construct(_soundService, faction, weaponData.Config, weaponData.Layer);
            return instance;
        }

        private WeaponData GetWeaponData(Faction faction, WeaponType type)
        {
            WeaponData weaponData;
            switch (faction)
            {
                case Faction.Player:
                    weaponData = _weaponDataPresenter.GetWeaponData(type, Constants.CacheContext.AGE);
                    break;
                case Faction.Enemy:
                    weaponData = _weaponDataPresenter.GetWeaponData(type, Constants.CacheContext.BATTLE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }

            return weaponData;
        }

        public void Reclaim(Projectile proj)
        { 
            if (!_projectilesPools.TryGetValue((proj.Faction, proj.Type), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(proj.Faction, proj.Type)] = pool;
            }

            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _projectilesPools.Values)
            {
                while (pool.Count > 0)
                {
                    var proj = pool.Dequeue();
                    Destroy(proj.gameObject);
                }
            }
            _projectilesPools.Clear(); 
        }
    }
}