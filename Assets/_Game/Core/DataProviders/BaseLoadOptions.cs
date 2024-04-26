using System.Threading;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataProviders
{
    public class BaseLoadOptions
    {
        public Faction Faction;
        public int CacheContext;
        public float Health;
        public string PrefabKey;
        public float CoinsAmount;
        public CancellationToken CancellationToken;
    }
}