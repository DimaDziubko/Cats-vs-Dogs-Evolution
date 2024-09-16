using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Data;
using _Game.Core.DataProviders.Facade;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core._DataLoaders.UnitDataLoaders
{
    public class UniversalUnitDataLoader : IUniversalUnitDataLoader
    {
        private readonly IMyLogger _logger;
        private readonly IUnitDataLoader _unitDataLoader;
        private readonly IUserContainer _userContainer;

        public UniversalUnitDataLoader(
            IMyLogger logger, 
            IUnitDataLoader unitDataLoader, 
            IUserContainer userContainer)
        {
            _logger = logger;
            _unitDataLoader = unitDataLoader;
            _userContainer = userContainer;
        }

        public async UniTask<DataPool<UnitType, IUnitData>> LoadAsync(IEnumerable<WarriorConfig> configs, LoadContext context)
        {
            var raceState = _userContainer.State.RaceState;
            DataPool<UnitType, IUnitData> pool = new DataPool<UnitType, IUnitData>();

            foreach (var config in configs)
            {
                var unitLoadOptions = new UnitLoadOptions
                {
                    Faction = context.Faction,
                    Config = config,
                    Timeline = context.Timeline,
                    CacheContext = context.CacheContext,
                    CurrentRace = raceState.CurrentRace
                };

                var data = await _unitDataLoader.LoadUnitDataAsync(unitLoadOptions);
                
                pool.Add(config.Type, data);
            }

            return pool;
        }
    }
}