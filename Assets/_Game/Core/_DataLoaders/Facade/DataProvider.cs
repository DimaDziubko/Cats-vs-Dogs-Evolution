using System.Collections.Generic;
using _Game.Core._DataLoaders.BaseDataLoader;
using _Game.Core._DataLoaders.EnvironmentDataLoader;
using _Game.Core._DataLoaders.UnitDataLoaders;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Ambience;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using Assets._Game.Gameplay._Bases.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.Facade
{
    public class DataLoaderFacade : IDataLoaderFacade
    {
        private readonly IUniversalUnitDataLoader _unitDataLoader;
        private readonly IUniversalWeaponDataLoader _weaponDataLoader;
        private readonly IBaseStaticDataLoader _baseStaticDataLoader;
        private readonly IEnvironmentDataLoader _environmentDataLoader;
        private readonly IAmbienceDataLoader _ambienceDataLoader;

        public DataLoaderFacade(
            IUniversalUnitDataLoader unitDataLoader,
            IUniversalWeaponDataLoader weaponDataLoader,
            IBaseStaticDataLoader baseStaticDataLoader,
            IEnvironmentDataLoader environmentDataLoader,
            IAmbienceDataLoader ambienceDataLoader)
        {
            _unitDataLoader = unitDataLoader;
            _weaponDataLoader = weaponDataLoader;
            _baseStaticDataLoader = baseStaticDataLoader;
            _environmentDataLoader = environmentDataLoader;
            _ambienceDataLoader = ambienceDataLoader;
        }
        
        public async UniTask<DataPool<UnitType, IUnitData>> LoadUnitsAsync(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitDataLoader.LoadAsync(configs, context);

        public DataPool<int, WeaponData> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context) => 
            _weaponDataLoader.Load(configs, context);
        
        public async UniTask<BaseStaticData> LoadBase(BaseLoadOptions options) => 
            await _baseStaticDataLoader.Load(options);
        
        public async UniTask<EnvironmentData> LoadEnvironment(string key, LoadContext context) => 
            await _environmentDataLoader.Load(key, context);

        public async UniTask<AudioClip> LoadAmbience(string key, LoadContext context) => 
            await _ambienceDataLoader.Load(key, context);
        
    }
}