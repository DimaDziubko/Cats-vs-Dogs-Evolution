using System.Collections.Generic;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.WeaponDataProviders
{
    public interface IUniversalWeaponDataProvider
    {
        UniTask<DataPool<WeaponType, WeaponData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}