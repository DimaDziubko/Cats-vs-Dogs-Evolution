using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;

namespace _Game.Gameplay.Vfx.Factory
{
    public interface IVfxFactory
    {
        public UnitBlot GetUnitBlot();
        public UnitExplosion GetUnitExplosion();
        public TowerSmoke GetBaseSmoke();
        void Reclaim(VfxType type, VfxEntity entity);
        void Reclaim(WeaponType type, MuzzleFlash muzzleFlash);
        void Reclaim(WeaponType type, ProjectileExplosion projectileExplosion);
        MuzzleFlash GetMuzzleFlash(Faction faction, WeaponType weaponType);
        ProjectileExplosion GetProjectileExplosion(Faction faction, WeaponType weaponType);
    }
}