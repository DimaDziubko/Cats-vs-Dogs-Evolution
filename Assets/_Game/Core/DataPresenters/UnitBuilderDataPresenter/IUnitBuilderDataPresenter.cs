using System;
using System.Collections.Generic;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataPresenters.UnitBuilderDataPresenter
{
    public interface IUnitBuilderDataPresenter
    {
        event Action<Dictionary<UnitType, UnitBuilderBtnModel>> BuilderModelUpdated;
        void OnBuilderStarted();
    }
}