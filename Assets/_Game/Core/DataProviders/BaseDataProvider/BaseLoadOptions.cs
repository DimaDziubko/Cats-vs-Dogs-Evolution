using System.Threading;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataProviders.BaseDataProvider
{
    public class BaseLoadOptions
    {
        public Faction Faction;
        public int CacheContext;
        public string PrefabKey;
        public float CoinsAmount;
    }
}