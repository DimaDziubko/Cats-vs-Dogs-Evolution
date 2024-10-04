using System;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core._DataPresenters.Evolution
{
    public interface IEvolutionPresenter
    {
        event Action<EvolutionTabModel> EvolutionModelUpdated;
        event Action LastAgeOpened;
        void OpenNextAge();
        void OnEvolutionTabOpened();
        float GetEvolutionPrice();
    }
}