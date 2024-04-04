using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Utils.Extensions
{
    public static class BattleConfigExtensions
    {
        public static string GetBaseKeyForAnotherRace( this BattleConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.EnemyBaseKey;
                case Race.Cat:
                    return config.EnemyBaseKey;
                case Race.Dog:
                    return config.PlayerBaseKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
    }
}