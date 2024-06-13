﻿using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.UnitBuilderDataProvider
{
    public interface IUnitBuilderDataProvider
    {
        UniTask<DataPool<UnitType, UnitBuilderBtnStaticData>> Load(IEnumerable<WarriorConfig> configs, LoadContext context);
    }
}