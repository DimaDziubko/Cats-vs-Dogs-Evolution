using System;
using Assets._Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace Assets._Game.Core.DataPresenters.TimelineTravel
{
    public interface ITimelineTravelPresenter
    {
        event Action<TravelTabModel> TravelViewModelUpdated;
        void OnTravelTabOpened();
        bool IsTimeToTravel();
        void OpenNextTimeline();
    }
}