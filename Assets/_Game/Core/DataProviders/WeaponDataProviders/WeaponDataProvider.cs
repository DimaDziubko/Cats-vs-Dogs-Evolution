using _Game.Core.AssetManagement;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Debugger;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Utils;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponDataProvider : IWeaponDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public WeaponDataProvider(
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask<WeaponData> LoadWeapon(WeaponLoadOptions options)
        {
            var projectilePrefab = await LoadComponent<Projectile>(options.Config.ProjectileKey, options.context);
            var muzzlePrefab = await LoadComponent<MuzzleFlash>(options.Config.MuzzleKey, options.context);
            var projectileExplosionPrefab = await LoadComponent<ProjectileExplosion>(options.Config.ProjectileExplosionKey, options.context);

            _logger.Log($"Weapon with id {options.Config.Id} load successfully");
            
            return new WeaponData()
            {
                Config = options.Config,
                Layer = options.Config.GetProjectileLayerForFaction(options.Faction),
                ProjectilePrefab = projectilePrefab,
                MuzzlePrefab = muzzlePrefab,
                ProjectileExplosionPrefab = projectileExplosionPrefab,
            };
        }

        private async UniTask<T> LoadComponent<T>(string key, LoadContext context) where T : Component
        {
            T component = null;
            if (key != Constants.ConfigKeys.MISSING_KEY)
            {
                var gameObject = await _assetRegistry.LoadAsset<GameObject>(key, context.Timeline, context.CacheContext);
                if (gameObject != null)
                {
                    component = gameObject.GetComponent<T>();
                }
            }
            return component;
        }
    }
}