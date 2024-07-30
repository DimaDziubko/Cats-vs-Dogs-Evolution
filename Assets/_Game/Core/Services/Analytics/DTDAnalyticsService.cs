using System;
using _Game.Core._GameInitializer;
using _Game.Core.Ads;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Battle.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class DTDAnalyticsService : IDTDAnalyticsService, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IAdsService _adsService;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        
        public DTDAnalyticsService(
            IUserContainer userContainer,
            IAdsService adsService,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _adsService = adsService;
            _gameInitializer = gameInitializer;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.OpenedUnit += OnUnitOpened;
            _adsService.AdImpression += TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged += OnStepCompleted;
            RaceState.Changed += OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            
            int zeroTutorialStepNumber = -1;
            if (TutorialState.StepsCompleted == zeroTutorialStepNumber)
            {
                int tutorialStartedKey = -1;
                DTDAnalytics.Tutorial(tutorialStartedKey);
            }
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.OpenedUnit -= OnUnitOpened;
            _adsService.AdImpression -= TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            RaceState.Changed -= OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnStepCompleted(int step)
        {
            var trueStepNumber = step + 1;
            int lastStep = 5;
            if (trueStepNumber == lastStep)
            {
                int tutorialCompleteKey = -2;
                DTDAnalytics.Tutorial( tutorialCompleteKey);
                return;
            }
            DTDAnalytics.Tutorial(trueStepNumber);
        }

        private void TrackRewardedVideoAdImpression(AdImpressionDto dto) => 
            DTDAnalytics.AdImpression(dto.Network, dto.Revenue, dto.Placement.ToString(), dto.UnitId);
        
        private void OnNextAgeOpened()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
        
            DTDAnalytics.CustomEvent("evolution_completed", parameters);
        }
        
        private void OnNextBattleOpened()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("battle№", TimelineState.MaxBattle);
        
            DTDAnalytics.CustomEvent("battle_completed", parameters);
        }
        
        public void OnBattleStarted(BattleAnalyticsData battleAnalyticsData)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", battleAnalyticsData.TimelineNumber);
            parameters.Add("age№", battleAnalyticsData.AgeNumber);
            parameters.Add("battle№", battleAnalyticsData.BattleNumber);
        
            DTDAnalytics.CustomEvent("battle_started", parameters);
        }
        
        public void SendEvent(string eventName)
        {
            DTDAnalytics.CustomEvent(eventName);
        }
        
        private void OnUnitOpened(UnitType type)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("unit", (int)type);
        
            DTDAnalytics.CustomEvent("unit_opened", parameters);
        }
        
        private void OnRaceChanged()
        {
            var parameters = new DTDCustomEventParameters();
            
            if (RaceState.Counter == 1)
            {
                DTDAnalytics.CustomEvent($"race_selected {RaceState.CurrentRace.ToString()}");
                _logger.Log("race_selected");
                return;
            }
        
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("race", RaceState.CurrentRace.ToString());
            DTDAnalytics.CustomEvent("race_changed", parameters);
            _logger.Log("race_changed");
        }
        
        private void OnCompletedBattleChanged()
        {
            if (BattleStatistics.BattlesCompleted == 1 && TutorialState.StepsCompleted == 1)
            {
                DTDAnalytics.CustomEvent("first_build_success");
                _logger.Log("first_build_success");
            }
            
            else if (BattleStatistics.BattlesCompleted == 1 && TutorialState.StepsCompleted == 0)
            {
                DTDAnalytics.CustomEvent("first_build_failed");
                _logger.Log("first_build_failed");
            }
        }
    }
}
