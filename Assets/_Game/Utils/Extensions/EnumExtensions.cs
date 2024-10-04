using System.Text.RegularExpressions;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string ToName(this BoostSource boostSource)
        {
            return Regex.Replace(boostSource.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
        
        public static string ToName(this BoostType boostSource)
        {
            return Regex.Replace(boostSource.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
        
        public static Race OppositeRace(this Race race)
        {
            return race == Race.Cat ? Race.Dog : Race.Cat;
        }
    }
}