using System.Collections.Generic;
using _Game.UI._MainMenu.Scripts;

namespace Assets._Game.Core._UpgradesChecker
{
    public interface IUpgradeAvailabilityProvider
    {
        IEnumerable<Screen> AffectedScreens { get; }
        bool IsAvailable { get; }
    }
}