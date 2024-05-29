using System;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Evolution.Scripts
{
    public interface IEvolutionService
    {
        UniTask Init();
        event Action<EvolutionTabData> EvolutionViewModelUpdated;
        event Action<TravelTabData> TravelViewModelUpdated;
        event Action<TimelineInfoData> TimelineInfoDataUpdated;
        event Action LastAgeOpened;
        void MoveToNextAge();
        bool IsTimeToTravel();
        void OnEvolutionTabOpened();
        void OnTimelineInfoWindowOpened();
        void OnTravelTabOpened();
        void MoveToNextTimeline();
    }
}