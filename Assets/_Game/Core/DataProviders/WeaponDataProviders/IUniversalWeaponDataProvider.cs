using System.Collections.Generic;
using _Game.Core.Configs.Models;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public interface IUniversalWeaponDataProvider
    {
        UniTask<DataPool<int, WeaponData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}