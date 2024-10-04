using System;
using _Game.Core.Configs.Models;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Utils.Extensions
{
    public static class WeaponConfigExtension
    {
        public static int GetProjectileLayerForFaction(this WeaponConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return Constants.Layer.PLAYER_PROJECTILE;
                case Faction.Enemy:
                    return Constants.Layer.ENEMY_PROJECTILE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            } 
        }
        
        public static float GetProjectileDamageForFaction(this WeaponConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return config.Damage * config.PlayerDamageMultiplier;
                case Faction.Enemy:
                    return config.Damage * config.EnemyDamageMultiplier;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            } 
        }
    }
}