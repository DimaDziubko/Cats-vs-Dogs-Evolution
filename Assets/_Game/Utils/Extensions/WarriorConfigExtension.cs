using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Utils.Extensions
{
    public static class WarriorConfigExtension
    {
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
    }
}