using System.Collections.Generic;
using _Game.Core._DataLoaders.UnitDataLoaders;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Ambience;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.EnvironmentDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Core.DataProviders.ShopDataProvider;
using _Game.Core.DataProviders.UnitBuilderDataProvider;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using _Game.UI._Shop.Scripts;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.Facade
{
    public class DataLoaderFacade : IDataLoaderFacade
    {
        private readonly IUniversalUnitDataLoader _unitDataLoader;
        private readonly IUniversalWeaponDataLoader _weaponDataLoader;
        private readonly IUnitBuilderDataLoader _unitBuilderDataLoader;
        private readonly IBaseStaticDataLoader _baseStaticDataLoader;
        private readonly IEnvironmentDataLoader _environmentDataLoader;
        private readonly IAmbienceDataLoader _ambienceDataLoader;
        private readonly IShopDataLoader _shopDataLoader;

        public DataLoaderFacade(
            IUniversalUnitDataLoader unitDataLoader,
            IUniversalWeaponDataLoader weaponDataLoader,
            IUnitBuilderDataLoader unitBuilderDataLoader,
            IBaseStaticDataLoader baseStaticDataLoader,
            IEnvironmentDataLoader environmentDataLoader,
            IAmbienceDataLoader ambienceDataLoader,
            IShopDataLoader shopDataLoader)
        {
            _unitDataLoader = unitDataLoader;
            _weaponDataLoader = weaponDataLoader;
            _unitBuilderDataLoader = unitBuilderDataLoader;
            _baseStaticDataLoader = baseStaticDataLoader;
            _environmentDataLoader = environmentDataLoader;
            _ambienceDataLoader = ambienceDataLoader;
            _shopDataLoader = shopDataLoader;
        }
        
        public async UniTask<DataPool<UnitType, IUnitData>> LoadUnitsAsync(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitDataLoader.LoadAsync(configs, context);

        public DataPool<int, WeaponData> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context) => 
            _weaponDataLoader.Load(configs, context);

        public async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitBuilderDataLoader.Load(configs, context);
        
        public async UniTask<BaseStaticData> LoadBase(BaseLoadOptions options) => 
            await _baseStaticDataLoader.Load(options);
        
        public async UniTask<EnvironmentData> LoadEnvironment(string key, LoadContext context) => 
            await _environmentDataLoader.Load(key, context);

        public async UniTask<AudioClip> LoadAmbience(string key, LoadContext context) => 
            await _ambienceDataLoader.Load(key, context);

        public async UniTask<DataPool<int, ShopItemStaticData>> LoadShopData() => 
            await _shopDataLoader.LoadShopData();
    }
}