using System.Collections.Generic;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.UnitUpgradeDataProvider
{
    public interface IUnitUpgradeDataProvider
    {
        UniTask<DataPool<UnitType, UnitUpgradeItemStaticData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}