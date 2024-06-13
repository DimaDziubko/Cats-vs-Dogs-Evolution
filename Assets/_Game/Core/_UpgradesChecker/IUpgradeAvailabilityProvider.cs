using System.Collections.Generic;
using _Game.UI._MainMenu.Scripts;

namespace _Game.Core._UpgradesChecker
{
    public interface IUpgradeAvailabilityProvider
    {
        IEnumerable<Window> AffectedWindows { get; }
        bool IsAvailable { get; }
    }
}