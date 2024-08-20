using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Debugger;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.Utils;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Utils;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponDataProvider : IWeaponDataProvider
    {
        private readonly IMyLogger _logger;

        public WeaponDataProvider(
            IMyLogger logger)
        {
            _logger = logger;
        }

        public WeaponData LoadWeapon(WeaponLoadOptions options)
        {
            return new WeaponData()
            {
                Config = options.Config,
                Layer = options.Config.GetProjectileLayerForFaction(options.Faction),
            };
        }
    }
}