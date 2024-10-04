using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Weapon.Scripts
{
    public interface IWeaponData
    {
        Projectile ProjectilePrefab { get;}
        MuzzleFlash MuzzlePrefab { get;}
        ProjectileExplosion ProjectileExplosionPrefab { get;}
        int Layer { get;}
        string ProjectileKey { get;}
        int WeaponId { get;}
        float ProjectileSpeed { get;}
        float TrajectoryWarpFactor { get; }
        float SplashRadius { get;}
        string ProjectileExplosionKey { get;}
        string MuzzleKey { get;}
        float GetProjectileDamageForFaction(Faction faction);
    }
}