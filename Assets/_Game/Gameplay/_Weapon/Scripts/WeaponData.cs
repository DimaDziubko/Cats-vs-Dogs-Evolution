using _Game.Core.Configs.Models;
using _Game.Gameplay.Vfx.Scripts;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class WeaponData
    {
        public WeaponConfig Config;
        public int Layer;
        public Projectile ProjectilePrefab;
        public MuzzleFlash MuzzlePrefab;
        public ProjectileExplosion ProjectileExplosionPrefab;
    }
}