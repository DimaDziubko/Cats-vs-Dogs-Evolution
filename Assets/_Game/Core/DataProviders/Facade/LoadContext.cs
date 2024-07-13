using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.DataProviders.Facade
{
    public struct LoadContext
    {
        public Faction Faction;
        public Race Race;
        public int Timeline;
        public int CacheContext;
    }
}