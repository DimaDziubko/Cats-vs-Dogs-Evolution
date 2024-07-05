using System.Collections.Generic;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.BaseDataProvider;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI._Environment;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.Facade
{
    public interface IDataProviderFacade
    {
        UniTask<DataPool<UnitType, UnitData>> LoadUnits(IEnumerable<WarriorConfig> configs, LoadContext context);
        UniTask<DataPool<WeaponType, WeaponData>> LoadWeapons(IEnumerable<WarriorConfig> configs, LoadContext context);
        UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> LoadUnitBuilderData(IEnumerable<WarriorConfig> configs, LoadContext context);
        UniTask<BaseStaticData> LoadBase(BaseLoadOptions options);
        UniTask<EnvironmentData> LoadEnvironment(string key, int cacheContext);
        UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> LoadUnitUpgradeItems(IEnumerable<WarriorConfig> configs, LoadContext cacheContext);
        UniTask<AudioClip> LoadAmbience(string configAmbienceKey, int cacheContext);
        UniTask<DataPool<Race, Sprite>> LoadFoodIcons(ICommonItemsConfigRepository itemsConfigRepository, int context);
        UniTask<Sprite> LoadTowerIcon(ICommonItemsConfigRepository itemsConfigRepository, int context);
    }
}