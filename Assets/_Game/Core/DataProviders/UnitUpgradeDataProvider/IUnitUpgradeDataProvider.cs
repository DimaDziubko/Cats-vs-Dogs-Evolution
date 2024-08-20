using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.UnitUpgradeDataProvider
{
    public interface IUnitUpgradeDataProvider
    {
        UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}