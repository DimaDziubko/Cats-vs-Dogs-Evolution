using _Game.Core._Logger;
using _Game.Core.DataProviders.WeaponDataProviders;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils.Extensions;

namespace _Game.Core.DataLoaders.WeaponDataProviders
{
    public class WeaponDataLoader : IWeaponDataLoader
    {
        private readonly IMyLogger _logger;

        public WeaponDataLoader(
            IMyLogger logger)
        {
            _logger = logger;
        }

        public WeaponData LoadWeapon(WeaponLoadOptions options)
        {
            return new WeaponData(options.Config)
            {
                Layer = options.Config.GetProjectileLayerForFaction(options.Faction),
            };
        }
    }
}