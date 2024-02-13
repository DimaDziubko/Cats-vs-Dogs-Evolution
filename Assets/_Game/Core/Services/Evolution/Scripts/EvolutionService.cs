using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

namespace _Game.Core.Services.Evolution.Scripts
{
    public class EvolutionService : IEvolutionService
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IMyLogger _logger;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;
        
        public EvolutionService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _logger = logger;
        }
        
        public void MoveToNextAge()
        {
            //TODO Implement later
            throw new System.NotImplementedException();
        }

        public int GetTimelineNumber() => TimelineState.TimelineId;

        public bool IsNextAgeAvailable()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId) <= Currency.Coins;
        }

        public float GetEvolutionPrice()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId);
        }
    }
}