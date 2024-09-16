using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.BaseDataProvider;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Environment;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core._DataLoaders.Facade
{
    public interface IDataLoaderFacade
    {
        UniTask<DataPool<UnitType, IUnitData>> LoadUnitsAsync(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context);
        DataPool<int, WeaponData> LoadWeapons(
            IEnumerable<WarriorConfig> configs, 
            LoadContext context);
        UniTask<BaseStaticData> LoadBase(BaseLoadOptions options);
        UniTask<EnvironmentData> LoadEnvironment(string key, LoadContext cacheContext);
        UniTask<AudioClip> LoadAmbience(string configAmbienceKey, LoadContext cacheContext);
    }
}