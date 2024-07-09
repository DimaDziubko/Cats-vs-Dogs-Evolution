using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using Assets._Game.Utils;
using Assets._Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponDataProvider : IWeaponDataProvider
    {
        private readonly IAssetRegistry _assetRegistry;

        public WeaponDataProvider(IAssetRegistry assetRegistry)
        {
            _assetRegistry = assetRegistry;
        }

        public async UniTask<WeaponData> LoadWeapon(WeaponLoadOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();
            
            var projectilePrefab = await LoadComponent<Projectile>(options.Config.ProjectileKey, options.CacheContext);
            var muzzlePrefab = await LoadComponent<MuzzleFlash>(options.Config.MuzzleKey, options.CacheContext);
            var projectileExplosionPrefab = await LoadComponent<ProjectileExplosion>(options.Config.ProjectileExplosionKey, options.CacheContext);

            
            return new WeaponData()
            {
                Config = options.Config,
                Layer = options.Config.GetProjectileLayerForFaction(options.Faction),
                ProjectilePrefab = projectilePrefab,
                MuzzlePrefab = muzzlePrefab,
                ProjectileExplosionPrefab = projectileExplosionPrefab,
            };
        }

        private async UniTask<T> LoadComponent<T>(string key, int cacheContext) where T : Component
        {
            T component = null;
            if (key != Constants.ConfigKeys.MISSING_KEY)
            {
                var gameObject = await _assetRegistry.LoadAsset<GameObject>(key, cacheContext);
                if (gameObject != null)
                {
                    component = gameObject.GetComponent<T>();
                }
            }
            return component;
        }
    }
}