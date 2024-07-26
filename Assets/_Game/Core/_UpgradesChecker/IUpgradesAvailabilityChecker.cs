using System;
using _Game.UI._MainMenu.Scripts;

namespace Assets._Game.Core._UpgradesChecker
{
    public interface IUpgradesAvailabilityChecker
    {
        event Action<NotificationData> Notify;
        NotificationData GetNotificationData(Screen screen);
        void Register(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void UnRegister(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void MarkAsReviewed(Screen screen);
    }
}