﻿using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Ambience;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Common;
using _Game.Core.DataProviders.EnvironmentDataProvider;
using _Game.Core.DataProviders.ShopDataProvider;
using _Game.Core.DataProviders.UnitBuilderDataProvider;
using _Game.Core.DataProviders.UnitDataProviders;
using _Game.Core.DataProviders.UnitUpgradeDataProvider;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using _Game.UI._Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
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
        private readonly IShopDataProvider _shopDataProvider;

        public DataProviderFacade(
            IUniversalUnitDataProvider unitDataProvider,
            IUniversalWeaponDataProvider weaponDataProvider,
            IUnitBuilderDataProvider unitBuilderDataProvider,
            IBaseStaticDataProvider baseStaticDataProvider,
            IEnvironmentDataProvider environmentDataProvider,
            IAmbienceDataProvider ambienceDataProvider,
            IUnitUpgradeDataProvider unitUpgradeDataProvider,
            ICommonItemsDataProvider commonItemDataProvider,
            IShopDataProvider shopDataProvider)
        {
            _unitDataProvider = unitDataProvider;
            _weaponDataProvider = weaponDataProvider;
            _unitBuilderDataProvider = unitBuilderDataProvider;
            _baseStaticDataProvider = baseStaticDataProvider;
            _environmentDataProvider = environmentDataProvider;
            _ambienceDataProvider = ambienceDataProvider;
            _unitUpgradeDataProvider = unitUpgradeDataProvider;
            _commonItemDataProvider = commonItemDataProvider;
            _shopDataProvider = shopDataProvider;
        }
        
        public async UniTask<DataPool<UnitType, UnitData>> LoadUnits(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitDataProvider.Load(configs, context);

        public async UniTask<DataPool<int, WeaponData>> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context) => 
            await _weaponDataProvider.Load(configs, context);

        public async UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(
            IEnumerable<WarriorConfig> configs,
            LoadContext context) => 
            await _unitBuilderDataProvider.Load(configs, context);
        
        public async UniTask<BaseStaticData> LoadBase(BaseLoadOptions options) => 
            await _baseStaticDataProvider.Load(options);
        
        public async UniTask<EnvironmentData> LoadEnvironment(string key, LoadContext context) => 
            await _environmentDataProvider.Load(key, context);

        public async UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(
            IEnumerable<WarriorConfig> configs, 
            LoadContext cacheContext) => 
            await _unitUpgradeDataProvider.Load(configs, cacheContext);

        public async UniTask<AudioClip> LoadAmbience(string key, LoadContext context) => 
            await _ambienceDataProvider.Load(key, context);

        public async UniTask<DataPool<Race, Sprite>> LoadFoodIcons(LoadContext context) => 
            await _commonItemDataProvider.LoadFoodIcons(context);
        
        public async UniTask<Sprite> LoadBaseIcon(LoadContext context) => 
            await _commonItemDataProvider.LoadBaseIcon(context);
        
        public async UniTask<DataPool<int, ShopItemStaticData>> LoadShopData() => 
            await _shopDataProvider.LoadShopData();
    }
}