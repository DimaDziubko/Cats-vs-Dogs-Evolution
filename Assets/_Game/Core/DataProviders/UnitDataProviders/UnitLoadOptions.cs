using System.Threading;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Core.DataProviders.UnitDataProviders
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