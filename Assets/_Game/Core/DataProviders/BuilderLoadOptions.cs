using System.Threading;
using _Game.Core.Configs.Models;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Core.DataProviders
{
    public class BuilderLoadOptions
    {
        public WarriorConfig Config;
        public Race CurrentRace;
        public CancellationToken CancellationToken;
    }
}