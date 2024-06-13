using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.DataProviders.BaseDataProvider;
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