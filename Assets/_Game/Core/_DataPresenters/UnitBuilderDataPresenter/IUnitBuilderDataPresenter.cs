﻿using System;
using System.Collections.Generic;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public interface IUnitBuilderDataPresenter
    {
        event Action<Dictionary<UnitType, UnitBuilderBtnModel>> BuilderModelUpdated;
        void OnBuilderStarted();
    }
}