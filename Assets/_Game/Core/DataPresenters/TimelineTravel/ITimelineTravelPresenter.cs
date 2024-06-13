using System;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.DataPresenters.TimelineTravel
{
    public interface ITimelineTravelPresenter
    {
        event Action<TravelTabModel> TravelViewModelUpdated;
        void OnTravelTabOpened();
        bool IsTimeToTravel();
        void OpenNextTimeline();
    }
}