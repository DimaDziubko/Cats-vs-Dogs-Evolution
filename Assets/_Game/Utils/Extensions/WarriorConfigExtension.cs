using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Utils.Extensions
{
    public static class WarriorConfigExtension
    {
        public static float GetUnitHealthForFaction(this WarriorConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return config.Health * config.PlayerHealthMultiplier;
                case Faction.Enemy:
                    return config.Health * config.EnemyHealthMultiplier;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
        
        public static string GetUnitKeyForCurrentRace( this WarriorConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.PlayerKey;
                case Race.Cat:
                    return config.PlayerKey;
                case Race.Dog:
                    return config.EnemyKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
        
        public static string GetUnitIconKeyForRace( this WarriorConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.PlayerIconKey;
                case Race.Cat:
                    return config.PlayerIconKey;
                case Race.Dog:
                    return config.EnemyIconKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
        
        public static string GetUnitKeyForAnotherRace( this WarriorConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.EnemyKey;
                case Race.Cat:
                    return config.EnemyKey;
                case Race.Dog:
                    return config.PlayerKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }

        public static int GetUnitLayerForFaction(this WarriorConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return Constants.Layer.PLAYER;
                case Faction.Enemy:
                    return Constants.Layer.ENEMY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
        public static int GetAggroLayerForFaction(this WarriorConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return Constants.Layer.PLAYER_AGGRO;
                case Faction.Enemy:
                    return Constants.Layer.ENEMY_AGGRO;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
        
        public static int GetAttackLayerForFaction(this WarriorConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return Constants.Layer.PLAYER_ATTACK;
                case Faction.Enemy:
                    return Constants.Layer.ENEMY_ATTACK;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            } 
        }
    }
}