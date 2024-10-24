﻿using System;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;

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
                    return config.CatKey;
                case Race.Cat:
                    return config.CatKey;
                case Race.Dog:
                    return config.DogKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
        
        public static string GetUnitIconNameForRace( this WarriorConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.CatIconName;
                case Race.Cat:
                    return config.CatIconName;
                case Race.Dog:
                    return config.DogIconName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }
        
        public static string GetUnitKeyForAnotherRace( this WarriorConfig config, Race race)
        {
            switch (race)
            {
                case Race.None:
                    return config.DogKey;
                case Race.Cat:
                    return config.DogKey;
                case Race.Dog:
                    return config.CatKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }

        public static int GetUnitLayerForFaction(this WarriorConfig config, Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    if(config.WeaponConfig.WeaponType == WeaponType.Melee) return Constants.Layer.MELEE_PLAYER;
                    else return Constants.Layer.RANGE_PLAYER;
                case Faction.Enemy:
                    if(config.WeaponConfig.WeaponType == WeaponType.Melee) return Constants.Layer.MELEE_ENEMY;
                    else return Constants.Layer.RANGE_ENEMY;
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