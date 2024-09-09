using _Game.Core.Configs.Models;
using _Game.Gameplay.Vfx.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Utils.Extensions;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class WeaponData : IWeaponData
    {
        private readonly WeaponConfig _config;

        public WeaponData(WeaponConfig config)
        {
            _config = config;
        }
        
        public int Layer { get; set; }
        public Projectile ProjectilePrefab { get; set; }
        public MuzzleFlash MuzzlePrefab { get; set; }
        public ProjectileExplosion ProjectileExplosionPrefab { get; set; }
        public string ProjectileKey => _config.ProjectileKey;
        public int WeaponId => _config.Id;
        public float ProjectileSpeed => _config.ProjectileSpeed;
        public float TrajectoryWarpFactor => _config.TrajectoryWarpFactor;
        public float SplashRadius => _config.SplashRadius;
        public string ProjectileExplosionKey => _config.ProjectileExplosionKey;
        public string MuzzleKey => _config.MuzzleKey;

        public float GetProjectileDamageForFaction(Faction faction) => 
            _config.GetProjectileDamageForFaction(faction);
    }
}