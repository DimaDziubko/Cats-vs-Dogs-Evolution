using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.UserState;
using Assets._Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.DataPresenters.TimelineTravel
{
    public class TimelineTravelPresenter : ITimelineTravelPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<TravelTabModel> TravelTabModelUpdated;
        IEnumerable<GameScreen> IUpgradeAvailabilityProvider.AffectedScreens
        {
            get
            {
                yield return GameScreen.Evolution;
                yield return GameScreen.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable =>
            TimelineState.AllBattlesWon;
        
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private EvolutionTabModel _evolutionTabModel;
        private TimelineInfoModel _timelineInfoModel;


        public TimelineTravelPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _userContainer = userContainer;
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            _upgradesChecker.Register(this);
            UpdateTravelData();
            _ageNavigator.AgeChanged += OnAgeChanged;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            _ageNavigator.AgeChanged -= OnAgeChanged;
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            _gameInitializer.OnMainInitialization-= Init;
        }

        private void OnNextTimelineOpened() => UpdateTravelData();

        private void OnAgeChanged() => UpdateTravelData();


        public void OpenNextTimeline() => 
            _userContainer.TimelineStateHandler.OpenNewTimeline();

        bool ITimelineTravelPresenter.IsTimeToTravel() => !IsNextAge();

        void ITimelineTravelPresenter.OnTravelTabOpened() => UpdateTravelData();


        private void UpdateTravelData()
        {
            int timelineNumberOffset = 2;

            string hint = "Win battle 6 first";
            
            if (TimelineState.AllBattlesWon && !IsNextTimeline)
            {
                hint = "Coming soon...";    
            }
            
            var travelViewModel = new TravelTabModel()
            {
                NextTimelineNumber = TimelineState.TimelineId + timelineNumberOffset,
                CanTravel = TimelineState.AllBattlesWon && IsNextTimeline,
                Hint = hint,
            };
            
            TravelTabModelUpdated?.Invoke(travelViewModel);
        }

        private bool IsNextTimeline => true;
        
        private bool IsNextAge() => 
            TimelineState.AgeId < _timelineConfigRepository.LastAge();
    }
}