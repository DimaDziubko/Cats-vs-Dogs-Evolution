﻿using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.UnitDataProviders
{
    public class UniversalUnitDataProvider : IUniversalUnitDataProvider
    {
        private readonly IMyLogger _logger;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly IUserContainer _persistentData;

        public UniversalUnitDataProvider(
            IMyLogger logger, 
            IUnitDataProvider unitDataProvider, 
            IUserContainer userContainer)
        {
            _logger = logger;
            _unitDataProvider = unitDataProvider;
            _persistentData = userContainer;
        }

        public async UniTask<DataPool<UnitType, UnitData>> Load (IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            var raceState = _persistentData.State.RaceState;
            DataPool<UnitType, UnitData> pool = new DataPool<UnitType, UnitData>();

            foreach (var config in configs)
            {
                var unitLoadOptions = new UnitLoadOptions
                {
                    Faction = context.Faction,
                    Config = config,
                    CacheContext = context.CacheContext,
                    CurrentRace = raceState.CurrentRace
                };

                var data = await _unitDataProvider.LoadUnitData(unitLoadOptions);
                pool.Add(config.Type, data);
            }

            return pool;
        }
    }
}