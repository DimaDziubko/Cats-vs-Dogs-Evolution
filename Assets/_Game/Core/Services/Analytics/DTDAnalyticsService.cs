using System;
using _Game.Core.Ads;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay.Battle.Scripts;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class DTDAnalyticsService : IDTDAnalyticsService, IDisposable
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IAdsService _adsService;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;

        public DTDAnalyticsService(
            IPersistentDataService persistentData,
            IAdsService adsService)
        {
            _persistentData = persistentData;
            _adsService = adsService;
        }

        public void Init()
        {
            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            _adsService.RewardedAdImpression += TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged += OnStepCompleted;
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            _adsService.RewardedAdImpression -= TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
        }

        private void OnStepCompleted(int step) => 
            DTDAnalytics.Tutorial(step);

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
    }
}
