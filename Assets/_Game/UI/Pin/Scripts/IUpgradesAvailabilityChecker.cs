using System;
using System.Collections.Generic;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.UI.Pin.Scripts
{
    public interface IUpgradesAvailabilityChecker
    {
        void Init();

        event Action<NotificationData> Notify;
        
        NotificationData GetNotificationData(Window window);
        
        void MarkAsReviewed(Window window);
        void OnUpgradeUnitItemsUpdated(List<UpgradeUnitItemViewModel> models);
        void OnUpgradeUnitItemsUpdated(UpgradeItemViewModel upgradeItem);
        void OnEvolutionModelUpdated(EvolutionTabData model);
        void OnTravelModelUpdated(TravelTabData model);
    }
}