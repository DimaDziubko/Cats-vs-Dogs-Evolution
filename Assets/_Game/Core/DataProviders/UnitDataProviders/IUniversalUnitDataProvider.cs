using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataProviders.UnitDataProviders
{
    public interface IUniversalUnitDataProvider
    {
        DataPool<UnitType, UnitData> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}