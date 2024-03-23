using System;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.Services.Evolution.Scripts
{
    public class EvolutionService : IEvolutionService
    {
        public event Action<EvolutionViewModel> EvolutionViewModelUpdated;
        public event Action<TravelViewModel> TravelViewModelUpdated;
        public event Action LastAgeOpened;

        private readonly IPersistentDataService _persistentData;

        private readonly IGameConfigController _gameConfigController;

        private readonly IMyLogger _logger;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        private EvolutionViewModel _evolutionViewModel;

        public EvolutionService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _logger = logger;
        }

        public void Init()
        {
            TimelineState.NextAgeOpened += OnNextAgeOpened;
        }

        private void OnNextAgeOpened()
        {
            UpdateEvolutionData();
            UpdateTravelData();
            
            if(!IsNextAge()) LastAgeOpened?.Invoke();
        }

        public void MoveToNextAge()
        {
            if (IsNextAge())
            {
                _persistentData.OpenNextAge();
            }
            else
            {
                _persistentData.OpenNextTimeline();
            }
        }

        public void MoveToNextTimeline()
        {
            //TODO Implement later
        }

        public bool IsTimeToTravel() => !IsNextAge();

        public void OnEvolutionTabOpened() => UpdateEvolutionData();

        public void OnTravelTabOpened() => UpdateTravelData();

        private void UpdateTravelData()
        {
            var travelViewModel = new TravelViewModel()
            {
                NextTimelineNumber = GetTimelineId() + 2,
                CanTravel = TimelineState.AllBattlesWon,
            };
            
            TravelViewModelUpdated?.Invoke(travelViewModel);
        }

        private void UpdateEvolutionData()
        {
            var currentAgeConfig = _gameConfigController.GetAgeConfig(TimelineState.AgeId);

            AgeConfig nextAgeConfig;
            
            if (IsNextAge())
            {
                nextAgeConfig = _gameConfigController.GetAgeConfig(TimelineState.AgeId + 1);
            }
            else
            {
                nextAgeConfig = currentAgeConfig;
            }
            
            var evolutionButtonData = new EvolutionBtnData()
            {
                CanAfford = IsNextAgeAffordable(),
                Price = GetEvolutionPrice()
            };
            
            
            if (_evolutionViewModel == null)
            {
                _evolutionViewModel = new EvolutionViewModel()
                {
                    CurrentTimelineId = GetTimelineId(),
                    EvolutionBtnData = evolutionButtonData,
                    CurrentAgeName = currentAgeConfig.Name,
                    NextAgeName = nextAgeConfig.Name
                };
            }
            else
            {
                _evolutionViewModel.CurrentTimelineId = GetTimelineId();
                _evolutionViewModel.EvolutionBtnData = evolutionButtonData;
                _evolutionViewModel.CurrentAgeName = currentAgeConfig.Name;
                _evolutionViewModel.NextAgeName = nextAgeConfig.Name;
            }
            
            EvolutionViewModelUpdated?.Invoke(_evolutionViewModel);
        }

        private bool IsNextAge() => 
            TimelineState.AgeId < _gameConfigController.LastAge();

        private int GetTimelineId() => TimelineState.TimelineId;

        private bool IsNextAgeAffordable()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId) <= Currency.Coins;
        }

        private float GetEvolutionPrice()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId);
        }
    }
    
}