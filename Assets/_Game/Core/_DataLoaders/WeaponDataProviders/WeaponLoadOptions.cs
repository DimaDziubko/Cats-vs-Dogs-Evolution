using _Game.Core.Configs.Models;
using _Game.Core.DataProviders.Facade;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataProviders.WeaponDataProviders
{
    public class WeaponLoadOptions
    {
        public Faction Faction;
        public WeaponConfig Config;
        public int CacheContext;
        public LoadContext context;
    }
}