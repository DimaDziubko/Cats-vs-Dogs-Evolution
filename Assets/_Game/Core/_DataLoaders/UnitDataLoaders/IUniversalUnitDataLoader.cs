using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.UnitDataLoaders
{
    public interface IUniversalUnitDataLoader
    {
        UniTask<DataPool<UnitType, IUnitData>> LoadAsync(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}