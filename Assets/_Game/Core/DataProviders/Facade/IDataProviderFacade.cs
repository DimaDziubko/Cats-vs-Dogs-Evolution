﻿using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.BaseDataProvider;
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
    public interface IDataProviderFacade
    {
        UniTask<DataPool<UnitType, UnitData>> LoadUnits(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context);
        UniTask<DataPool<int, WeaponData>> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context);
        UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context);
        UniTask<BaseStaticData> LoadBase(BaseLoadOptions options);
        UniTask<EnvironmentData> LoadEnvironment(string key, LoadContext cacheContext);
        UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(
            IEnumerable<WarriorConfig> configs, 
            LoadContext cacheContext);
        UniTask<AudioClip> LoadAmbience(string configAmbienceKey, LoadContext cacheContext);
        UniTask<DataPool<Race, Sprite>> LoadFoodIcons(LoadContext cacheContext);
        UniTask<Sprite> LoadBaseIcon(LoadContext cacheContext);
        UniTask<DataPool<int, ShopItemStaticData>> LoadShopData();
    }
}