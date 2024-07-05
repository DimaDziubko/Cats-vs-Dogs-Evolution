using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace Assets._Game.Creatives.LocalUnitConfigs.Scr
{
    [CreateAssetMenu(fileName = "LocalUnitConfig", menuName = "LocalConfigs/Units", order = 0)]
    public class LocalUnitConfig : ScriptableObject
    {
        public UnitType Type;
        public UnitData Data;
        public Sprite Icon;
        public MuzzleFlash MuzzlePrefab;
        public Projectile ProjectilePrefab;
        public ProjectileExplosion ProjectileExplosionPrefab;
    }
}