using System;
using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Creatives.Factories
{
    [CreateAssetMenu(fileName = "CrProjectileFactory", menuName = "CrFactories/Projectile")]
    public class CrProjectileFactory : GameObjectFactory, IProjectileFactory
    {
        private ISoundService _soundService;

        private readonly Dictionary<(Faction, int), Queue<Projectile>> _projectilesPools =
            new Dictionary<(Faction, int), Queue<Projectile>>();

        public void Initialize(ISoundService soundService)
        {
            _soundService = soundService;
        }
        
        public Projectile Get(Faction faction,  int weaponId)
        {
            WeaponData weaponData = GetWeaponData(faction, weaponId);
            
            if (!_projectilesPools.TryGetValue((faction, weaponId), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(faction, weaponId)] = pool;
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

            instance.Construct(_soundService, faction, weaponData);
            return instance;
        }

        public UniTask<Projectile> GetAsync(Faction faction, int weaponId)
        {
            throw new NotImplementedException();
        }

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

        public void Reclaim(Projectile proj)
        { 
            if (!_projectilesPools.TryGetValue((proj.Faction, proj.WeaponId), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(proj.Faction, proj.WeaponId)] = pool;
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
    
