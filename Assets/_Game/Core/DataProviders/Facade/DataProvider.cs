﻿using System.Collections;
using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.DataProviders.Ambience;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Common;
using _Game.Core.DataProviders.EnvironmentDataProvider;
using _Game.Core.DataProviders.UnitBuilderDataProvider;
using _Game.Core.DataProviders.UnitDataProviders;
using _Game.Core.DataProviders.UnitUpgradeDataProvider;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Common.Scripts;
using _Game.UI._Environment;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.Facade
{
    public class DataProviderFacade : IDataProviderFacade
    {
        private readonly IUniversalUnitDataProvider _unitDataProvider;
        private readonly IUniversalWeaponDataProvider _weaponDataProvider;
        private readonly IUnitBuilderDataProvider _unitBuilderDataProvider;
        private readonly IBaseStaticDataProvider _baseStaticDataProvider;
        private readonly IEnvironmentDataProvider _environmentDataProvider;
        private readonly IAmbienceDataProvider _ambienceDataProvider;
        private readonly IUnitUpgradeDataProvider _unitUpgradeDataProvider;
        private readonly ICommonItemsDataProvider _commonItemDataProvider;

        public DataProviderFacade(
            IUniversalUnitDataProvider unitDataProvider,
            IUniversalWeaponDataProvider weaponDataProvider,
            IUnitBuilderDataProvider unitBuilderDataProvider,
            IBaseStaticDataProvider baseStaticDataProvider,
            IEnvironmentDataProvider environmentDataProvider,
            IAmbienceDataProvider ambienceDataProvider,
            IUnitUpgradeDataProvider unitUpgradeDataProvider,
            ICommonItemsDataProvider commonItemDataProvider)
        {
            _unitDataProvider = unitDataProvider;
            _weaponDataProvider = weaponDataProvider;
            _unitBuilderDataProvider = unitBuilderDataProvider;
            _baseStaticDataProvider = baseStaticDataProvider;
            _environmentDataProvider = environmentDataProvider;
            _ambienceDataProvider = ambienceDataProvider;
            _unitUpgradeDataProvider = unitUpgradeDataProvider;
            _commonItemDataProvider = commonItemDataProvider;
        }
        
        public async UniTask<DataPool<UnitType, UnitData>> LoadUnits(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitDataProvider.Load(configs, context);

        public async UniTask<DataPool<WeaponType, WeaponData>> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context) => 
            await _weaponDataProvider.Load(configs, context);

        public async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitBuilderDataProvider.Load(configs, context);
        
        public async UniTask<BaseStaticData> LoadBase(BaseLoadOptions options) => 
            await _baseStaticDataProvider.Load(options);
        
        public async UniTask<EnvironmentData> LoadEnvironment(string key, int cacheContext) => 
            await _environmentDataProvider.Load(key, cacheContext);

        public async UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(
            IEnumerable<WarriorConfig> configs, 
            LoadContext cacheContext) => 
            await _unitUpgradeDataProvider.Load(configs, cacheContext);

        public async UniTask<AudioClip> LoadAmbience(string key, int cacheContext) => 
            await _ambienceDataProvider.Load(key, cacheContext);

        public async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, int context) => 
            await _commonItemDataProvider.LoadFoodIcons(itemsConfigRepository, context);
        
        public async UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository, int context) => 
            await _commonItemDataProvider.LoadTowerIcon(itemsConfigRepository, context);
    }
}