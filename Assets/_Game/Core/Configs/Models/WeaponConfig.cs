using System;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core.Configs.Models
{
    [Serializable]
    public class WeaponConfig
    {
        public int Id;
        public WeaponType WeaponType;
        public float Damage;
        public float ProjectileSpeed;
        public string ProjectileKey;
        public float TrajectoryWarpFactor;
        public string MuzzleKey;
        public string ProjectileExplosionKey;
        public float SplashRadius;
        public float PlayerDamageMultiplier;
        public float EnemyDamageMultiplier;
    }
}