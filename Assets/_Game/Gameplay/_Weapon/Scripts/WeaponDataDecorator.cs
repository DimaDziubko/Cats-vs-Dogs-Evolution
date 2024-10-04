using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Weapon.Scripts
{
    public abstract class WeaponDataDecorator : IWeaponData
    {
        private readonly IWeaponData _weaponData;

        protected WeaponDataDecorator(IWeaponData weaponData)
        {
            _weaponData = weaponData;
        }

        public Projectile ProjectilePrefab => _weaponData.ProjectilePrefab;
        public MuzzleFlash MuzzlePrefab => _weaponData.MuzzlePrefab;
        public ProjectileExplosion ProjectileExplosionPrefab => _weaponData.ProjectileExplosionPrefab;
        public int Layer => _weaponData.Layer;
        public string ProjectileKey => _weaponData.ProjectileKey;
        public int WeaponId => _weaponData.WeaponId;
        public float ProjectileSpeed => _weaponData.ProjectileSpeed;
        public float TrajectoryWarpFactor => _weaponData.TrajectoryWarpFactor;
        public float SplashRadius => _weaponData.SplashRadius;
        public string ProjectileExplosionKey => _weaponData.ProjectileExplosionKey;
        public string MuzzleKey => _weaponData.MuzzleKey;

        public virtual float GetProjectileDamageForFaction(Faction faction) => 
            _weaponData.GetProjectileDamageForFaction(faction);
    }
}