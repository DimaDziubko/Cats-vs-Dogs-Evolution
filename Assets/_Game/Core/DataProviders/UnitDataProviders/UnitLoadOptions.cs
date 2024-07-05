using System.Threading;
using Assets._Game.Core.Configs.Models;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;

namespace Assets._Game.Core.DataProviders.UnitDataProviders
{
    public class UnitLoadOptions
    {
        public Faction Faction;
        public WarriorConfig Config;
        public int CacheContext;
        public Race CurrentRace;
        public CancellationToken CancellationToken;
    }
}