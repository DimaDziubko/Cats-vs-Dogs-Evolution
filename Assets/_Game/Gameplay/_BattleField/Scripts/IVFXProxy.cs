using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
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