using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core._DataLoaders.WeaponDataProviders
{
    public class UniversalWeaponDataLoader : IUniversalWeaponDataLoader
    {
        private readonly IMyLogger _logger;
        private readonly IWeaponDataLoader _weaponDataLoader;

        public UniversalWeaponDataLoader(
            IMyLogger logger,
            IWeaponDataLoader weaponDataLoader)
        {
            _logger = logger;
            _weaponDataLoader = weaponDataLoader;
        }
        
        
        public DataPool<int, WeaponData> Load(IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            DataPool<int, WeaponData> pool = new DataPool<int, WeaponData>();
            
            foreach (var config in configs)
            {
                if (config.WeaponConfig.WeaponType == WeaponType.Melee) continue;

                var weaponLoadOptions = new WeaponLoadOptions()
                {
                    Faction = context.Faction,
                    Config = config.WeaponConfig,
                    CacheContext = context.CacheContext,
                };
            
                WeaponData data =
                     _weaponDataLoader.LoadWeapon(weaponLoadOptions);
                
                pool.Add(config.WeaponConfig.Id,  data);
            }

            return pool;
        }
    }
}