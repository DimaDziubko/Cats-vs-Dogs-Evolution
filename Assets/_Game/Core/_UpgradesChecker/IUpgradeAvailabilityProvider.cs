using System.Collections.Generic;
using Assets._Game.UI._MainMenu.Scripts;

namespace Assets._Game.Core._UpgradesChecker
{
    public interface IUpgradeAvailabilityProvider
    {
        IEnumerable<Window> AffectedWindows { get; }
        bool IsAvailable { get; }
    }
}