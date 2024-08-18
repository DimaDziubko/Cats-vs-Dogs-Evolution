using System.Collections.Generic;
using _Game.UI._MainMenu.Scripts;

namespace Assets._Game.Core._UpgradesChecker
{
    public interface IUpgradeAvailabilityProvider
    {
        IEnumerable<GameScreen> AffectedScreens { get; }
        bool IsAvailable { get; }
    }
}