using _Game.Core.Configs.Models;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core._DataLoaders.UnitDataLoaders
{
    public class UnitLoadOptions
    {
        public Faction Faction;
        public WarriorConfig Config;
        public int CacheContext;
        public Race CurrentRace;
        public int Timeline;
    }
}