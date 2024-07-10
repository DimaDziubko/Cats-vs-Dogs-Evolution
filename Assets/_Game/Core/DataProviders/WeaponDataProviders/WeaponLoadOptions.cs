using System.Threading;
using _Game.Core.Configs.Models;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponLoadOptions
    {
        public Faction Faction;
        public WeaponConfig Config;
        public int CacheContext;
        public CancellationToken CancellationToken;
    }
}