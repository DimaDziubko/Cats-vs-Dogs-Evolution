﻿using System;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.DataPresenters.Evolution
{
    public interface IEvolutionPresenter
    {
        event Action<EvolutionTabModel> EvolutionModelUpdated;
        event Action LastAgeOpened;
        void OpenNextAge();
        void OnEvolutionTabOpened();
    }
}