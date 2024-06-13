using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public interface IUniversalWeaponDataProvider
    {
        UniTask<DataPool<WeaponType, WeaponData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}