﻿using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public interface IWeaponDataLoader
    {
        WeaponData LoadWeapon(WeaponLoadOptions options);
    }
}