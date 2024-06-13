using System.Threading;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponLoadOptions
    {
        public Faction Faction;
        public WeaponConfig Config;
        public int CacheContext;
        public CancellationToken CancellationToken;
    }
}