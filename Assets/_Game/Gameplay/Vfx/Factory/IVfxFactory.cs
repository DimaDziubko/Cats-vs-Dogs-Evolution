using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay.Vfx.Factory
{
    public interface IVfxFactory
    {
        public UnitBlot GetUnitBlot();
        public UnitExplosion GetUnitExplosion();
        public BaseSmoke GetBaseSmoke();
        void Reclaim(VfxType type, VfxEntity entity);
        void Reclaim(int weaponId, MuzzleFlash muzzleFlash);
        void Reclaim(int weaponId, ProjectileExplosion projectileExplosion);
        MuzzleFlash GetMuzzleFlash(Faction faction, int weaponId);
        ProjectileExplosion GetProjectileExplosion(Faction faction, int weaponId);
        UniTask<MuzzleFlash> GetMuzzleFlashAsync(Faction faction, int weaponId);
        UniTask<ProjectileExplosion> GetProjectileExplosionAsync(Faction faction, int weaponId);
    }
}