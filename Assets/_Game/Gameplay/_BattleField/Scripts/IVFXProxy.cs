using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IVFXProxy
    {
        void SpawnUnitVfx(Vector3 position);
        void SpawnMuzzleFlash(MuzzleData muzzleData);
        void SpawnProjectileExplosion(ExplosionData explosionData);
    }
}