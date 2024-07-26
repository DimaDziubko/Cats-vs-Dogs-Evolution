using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using Assets._Game.Core.Data;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.UnitDataProviders
{
    public interface IUniversalUnitDataProvider
    {
        UniTask<DataPool<UnitType, UnitData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}