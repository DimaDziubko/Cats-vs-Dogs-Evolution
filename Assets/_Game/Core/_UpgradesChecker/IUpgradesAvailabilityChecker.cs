using System;
using _Game.UI._MainMenu.Scripts;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.Core._UpgradesChecker
{
    public interface IUpgradesAvailabilityChecker
    {
        event Action<NotificationData> Notify;
        NotificationData GetNotificationData(GameScreen gameScreen);
        void Register(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void UnRegister(IUpgradeAvailabilityProvider unitUpgradesPresenter);
        void MarkAsReviewed(GameScreen gameScreen);
    }
}