using System.Collections.Generic;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.WeaponDataProviders
{
    public class UniversalWeaponDataProvider : IUniversalWeaponDataProvider
    {
        private readonly IMyLogger _logger;
        private readonly IWeaponDataProvider _weaponDataProvider;

        public UniversalWeaponDataProvider(
            IMyLogger logger,
            IWeaponDataProvider weaponDataProvider)
        {
            _logger = logger;
            _weaponDataProvider = weaponDataProvider;
        }
        
        
        public async UniTask<DataPool<WeaponType, WeaponData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            DataPool<WeaponType, WeaponData> pool = new DataPool<WeaponType, WeaponData>();
            
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
                    await _weaponDataProvider.LoadWeapon(weaponLoadOptions);
                
                pool.Add(config.WeaponConfig.WeaponType,  data);
            }

            return pool;
        }
    }
}