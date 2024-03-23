using System;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.Services.Evolution.Scripts
{
    public interface IEvolutionService
    {
        void Init();
        void MoveToNextAge();
        bool IsTimeToTravel();
        void OnEvolutionTabOpened();
        event Action<EvolutionViewModel> EvolutionViewModelUpdated;
        void OnTravelTabOpened();
        event Action<TravelViewModel> TravelViewModelUpdated;
        void MoveToNextTimeline();
        event Action LastAgeOpened;
    }
}