using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;

namespace Assets._Game.Gameplay.Vfx.Factory
{
    public interface IVfxFactory
    {
        public UnitBlot GetUnitBlot();
        public UnitExplosion GetUnitExplosion();
        public BaseSmoke GetBaseSmoke();
        void Reclaim(VfxType type, VfxEntity entity);
        void Reclaim(WeaponType type, MuzzleFlash muzzleFlash);
        void Reclaim(WeaponType type, ProjectileExplosion projectileExplosion);
        MuzzleFlash GetMuzzleFlash(Faction faction, WeaponType weaponType);
        ProjectileExplosion GetProjectileExplosion(Faction faction, WeaponType weaponType);
    }
}