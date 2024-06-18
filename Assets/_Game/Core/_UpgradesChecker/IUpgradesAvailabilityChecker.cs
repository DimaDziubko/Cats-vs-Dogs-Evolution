using System;
using _Game.UI._MainMenu.Scripts;

namespace _Game.Core._UpgradesChecker
{
    public interface IUpgradesAvailabilityChecker
    {
        event Action<NotificationData> Notify;
        NotificationData GetNotificationData(Window window);
        void Register(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void UnRegister(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void MarkAsReviewed(Window window);
    }
}