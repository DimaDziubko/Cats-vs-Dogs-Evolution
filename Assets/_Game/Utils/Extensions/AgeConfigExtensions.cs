using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Utils.Extensions
{
    public static class AgeConfigExtensions
    {
        public static string GetBaseKeyForRace( this AgeConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.PlayerBaseKey;
                case Race.Cat:
                    return config.PlayerBaseKey;
                case Race.Dog:
                    return config.EnemyBaseKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
    }
}