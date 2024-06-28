using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Pin.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;

namespace _Game.Core.DataPresenters.TimelineTravel
{
    public class TimelineTravelPresenter : ITimelineTravelPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<TravelTabModel> TravelViewModelUpdated;
        IEnumerable<Window> IUpgradeAvailabilityProvider.AffectedWindows
        {
            get
            {
                yield return Window.Evolution;
                yield return Window.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable =>
            TimelineState.AllBattlesWon;
        
        private readonly IUserContainer _persistentData;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private EvolutionTabModel _evolutionTabModel;
        private TimelineInfoModel _timelineInfoModel;


        public TimelineTravelPresenter(
            IUserContainer persistentData,
            ITimelineConfigRepository timelineConfigRepository,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _persistentData = persistentData;
            _timelineConfigRepository = timelineConfigRepository;
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
            //TimelineState.NextTimelineOpened += OnNextTimelineOpened;
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            _ageNavigator.AgeChanged -= OnAgeChanged;
            //TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            _gameInitializer.OnMainInitialization-= Init;
        }

        private void OnNextTimelineOpened() => UpdateTravelData();

        private void OnAgeChanged() => UpdateTravelData();


        public void OpenNextTimeline()
        {
            //TODO Implement later
        }

        bool ITimelineTravelPresenter.IsTimeToTravel() => !IsNextAge();

        void ITimelineTravelPresenter.OnTravelTabOpened() => UpdateTravelData();


        private void UpdateTravelData()
        {
            int timelineNumberOffset = 2;
            
            var travelViewModel = new TravelTabModel()
            {
                NextTimelineNumber = TimelineState.TimelineId + timelineNumberOffset,
                CanTravel = TimelineState.AllBattlesWon,
            };
            
            TravelViewModelUpdated?.Invoke(travelViewModel);
        }

        private bool IsNextAge() => 
            TimelineState.AgeId < _timelineConfigRepository.LastAge();
    }
}