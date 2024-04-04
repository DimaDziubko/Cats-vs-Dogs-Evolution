using System;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Evolution.Scripts
{
    public interface IEvolutionService
    {
        UniTask Init();
        void MoveToNextAge();
        bool IsTimeToTravel();
        void OnEvolutionTabOpened();
        void OnTimelineInfoWindowOpened();
        event Action<EvolutionViewModel> EvolutionViewModelUpdated;
        void OnTravelTabOpened();
        event Action<TravelViewModel> TravelViewModelUpdated;
        void MoveToNextTimeline();
        event Action LastAgeOpened;
        event Action<TimelineInfoData> TimelineInfoDataUpdated;
    }
}