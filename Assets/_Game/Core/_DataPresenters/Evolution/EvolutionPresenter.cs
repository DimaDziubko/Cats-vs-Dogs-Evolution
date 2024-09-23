using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.UserState;

namespace _Game.Core._DataPresenters.Evolution
{
    public class EvolutionPresenter : IEvolutionPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<EvolutionTabModel> EvolutionModelUpdated;
        public event Action LastAgeOpened;
        
        IEnumerable<GameScreen> IUpgradeAvailabilityProvider.AffectedScreens
        {
            get
            {
                yield return GameScreen.Evolution;
                yield return GameScreen.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable => IsNextAgeAffordable();

        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IDifficultyConfigRepository _difficultyConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IAgeNavigator _ageNavigator;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _userContainer.State.Currencies;

        private EvolutionTabModel _evolutionTabModel;

        public EvolutionPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _userContainer = userContainer;
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
            _difficultyConfigRepository = configRepositoryFacade.DifficultyConfigRepository;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            _upgradesChecker.Register(this);
            UpdateEvolutionData();
            _ageNavigator.AgeChanged += OnAgeChanged;
            Currency.CurrenciesChanged += OnCurrenciesChanged;
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.Register(this);
            TimelineState.NextAgeOpened -= OnAgeChanged;
            Currency.CurrenciesChanged -= OnCurrenciesChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        public void OpenNextAge()
        {
            if (IsNextAge())
            {
                _userContainer.TimelineStateHandler.OpenNewAge();
            }
            else
            {
                _userContainer.TimelineStateHandler.OpenNewTimeline();
            }
        }

        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source) 
            => UpdateEvolutionData();

        private void OnNextTimelineOpened() => UpdateEvolutionData();

        private void OnAgeChanged()
        {
            UpdateEvolutionData();
            if(!IsNextAge()) LastAgeOpened?.Invoke();
        }

        void IEvolutionPresenter.OnEvolutionTabOpened() => UpdateEvolutionData();

        private void UpdateEvolutionData()
        {
            int currentAgeNumber = TimelineState.AgeId;
            int nextAgeNumber = TimelineState.AgeId + 1;
            
            EvolutionBtnData evolutionButtonData = new EvolutionBtnData()
            {
                ButtonState = IsNextAgeAffordable() ? ButtonState.Active : ButtonState.Inactive,
                Price = GetEvolutionPrice()
            };

            _evolutionTabModel = new EvolutionTabModel()
            {
                CurrentTimelineId = TimelineState.TimelineId,
                CurrentAgeIcon = _generalDataPool.TimelineStaticData.TimelineInfoItems[currentAgeNumber].AgeIcon,
                EvolutionBtnData = evolutionButtonData,
                CurrentAgeName = _generalDataPool.TimelineStaticData.TimelineInfoItems[currentAgeNumber].Name,
            };

            if (IsNextAge())
            {
                _evolutionTabModel.NextAgeIcon =
                    _generalDataPool.TimelineStaticData.TimelineInfoItems[nextAgeNumber].AgeIcon;
                _evolutionTabModel.NextAgeName =
                    _generalDataPool.TimelineStaticData.TimelineInfoItems[nextAgeNumber].Name;
            }
            
            EvolutionModelUpdated?.Invoke(_evolutionTabModel);
        }

        private bool IsNextAge() => 
            TimelineState.AgeId < _timelineConfigRepository.LastAge();

        private bool IsNextAgeAffordable()
        {
            if (TimelineState.MaxBattle > TimelineState.AgeId)
            {
                return true;
            }
            return _difficultyConfigRepository
                .GetEvolutionPrice(TimelineState.TimelineId + 1, TimelineState.AgeId + 1) 
                   <= Currency.Coins;
        }

        public float GetEvolutionPrice()
        {
            if (TimelineState.MaxBattle > TimelineState.AgeId) return -1;
            return _difficultyConfigRepository
                .GetEvolutionPrice(TimelineState.TimelineId + 1, TimelineState.AgeId + 1);
        }
    }
    
}