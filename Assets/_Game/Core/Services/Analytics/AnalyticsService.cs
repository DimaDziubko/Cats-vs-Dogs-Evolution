using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.Gameplay._Battle.Scripts;
using Assets._Game.Core.Services.Analytics;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using MadPixelAnalytics;
using UnityEngine.Device;

namespace _Game.Core.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService, IDisposable
    {
        private FirebaseApp _app;
        private bool _isFirebaseInitialized = false;
        private string UniqueID { get; set; }

        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly AppMetricaComp _appMetricaComp;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        private IAdsStatisticsReadonly AdsStatistics => _userContainer.State.AdsStatistics;
        private IRetentionStateReadonly RetentionStateReadonly => _userContainer.State.RetentionState;

        public AnalyticsService(
            IMyLogger logger,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            AppMetricaComp appMetricaComp)
        {
            _logger = logger;
            _userContainer = userContainer;
            _appMetricaComp = appMetricaComp;
            gameInitializer.RegisterAsyncInitialization(Init);
        }

        private async UniTask Init()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                SetUniqID();

                _isFirebaseInitialized = true;
                _logger.Log("Firebase Initialized Successfully");
            }
            else
            {
                _logger.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }

            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.OpenedUnit += OnUnitOpened;
            RaceState.Changed += OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            AdsStatistics.AdsReviewedChanged += OnAdsStatisticsChanged;
            RetentionStateReadonly.FirstDayRetentionEventSentChanged += SendFirstDayRetentionEvent;
            RetentionStateReadonly.SecondDayRetentionEventSentChanged += SendSecondDayRetentionEvent;
            TutorialState.StepsCompletedChanged += OnStepCompleted;
        }

        private void SetUniqID()
        {
            UniqueID = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
            }
            _logger.Log($"Generated fallback Unique ID: {UniqueID}");
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.OpenedUnit -= OnUnitOpened;
            RaceState.Changed -= OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleChanged;
            AdsStatistics.AdsReviewedChanged -= OnAdsStatisticsChanged;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            _app?.Dispose();
        }

        private void SendFirstDayRetentionEvent()
        {
            if (IsComponentsReady()) return;
            SendEvent($"retention_1d");
        }

        private void SendSecondDayRetentionEvent()
        {
            if (IsComponentsReady()) return;
            SendEvent($"second_open");
        }

        private void OnCompletedBattleChanged()
        {
            if (!IsComponentsReady()) return;

            if (BattleStatistics.BattlesCompleted == 1 && TutorialState.StepsCompleted == 1)
            {
                SendEvent("first_build_success");
                _logger.Log("first_build_success");


            }

            else if (BattleStatistics.BattlesCompleted == 1 && TutorialState.StepsCompleted == 0)
            {
                SendEvent("first_build_failed");
                _logger.Log("first_build_failed");
            }
        }

        private void OnRaceChanged()
        {
            if (!IsComponentsReady()) return;

            if (RaceState.Counter == 1)
            {
                SendEvent($"race_selected_{RaceState.CurrentRace.ToString()}");
                return;
            }

            SendEvent($"race_changed_{RaceState.CurrentRace.ToString()}");
        }

        public void OnBattleStarted(BattleAnalyticsData battleAnalyticsData)
        {
            if (!IsComponentsReady()) return;
            SendEvent($"battle_started_{battleAnalyticsData.BattleNumber}");


            _appMetricaComp.SendCustomEvent("level_start", new Dictionary<string, object>() {
                {"level_number", battleAnalyticsData.BattleNumber },
                {"age_number", battleAnalyticsData.AgeNumber },
                {"timeline_number", battleAnalyticsData.TimelineNumber },
            }, true);
        }

        private bool IsComponentsReady() =>
            _isFirebaseInitialized || _userContainer != null;

        private void OnUnitOpened(UnitType type)
        {
            if (!IsComponentsReady()) return;
            SendEvent($"unit_opened_{type}");
        }

        private void OnNextBattleOpened()
        {
            if (!IsComponentsReady()) return;

            SendEvent($"battle_completed_{TimelineState.MaxBattle}");

            // SendEvent("battle_completed", new Dictionary<string, object>
            // {
            //     {"TimelineNumber", TimelineState.TimelineId + 1},
            //     {"AgeNumber", TimelineState.AgeId + 1},
            //     {"BattleNumber", TimelineState.MaxBattle}
            // });

            _appMetricaComp.SendCustomEvent("level_finish", new Dictionary<string, object>() {
                {"level_number", TimelineState.MaxBattle },
                {"age_number", TimelineState.AgeId},
                {"timeline_number", TimelineState.TimelineId},
            }, true);
        }

        private void OnNextAgeOpened()
        {
            if (!IsComponentsReady()) return;
            SendEvent($"evolution_completed_{TimelineState.AgeId}_timeline_{TimelineState.TimelineId + 1}");
        }

        private void OnAdsStatisticsChanged()
        {
            if (!IsComponentsReady()) return;
            SendEvent($"ad_impression_{AdsStatistics.AdsReviewed}");
        }

        private void SendEvent(string eventName, Dictionary<string, object> eventData)
        {
            if (_isFirebaseInitialized)
            {
                var analyticsEvent = new AnalyticsEvent(eventName);
                foreach (var kv in eventData)
                {
                    analyticsEvent.AddParameter(kv.Key, kv.Value);
                }

                analyticsEvent.Send();
            }
            else
            {
                _logger.LogWarning($"Firebase app not initialized. Cannot log event: {eventName}");
            }
        }

        public void SendEvent(string eventName)
        {
            if (_isFirebaseInitialized)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                _logger.LogWarning($"Firebase app not initialized. Cannot log event: {eventName}");
            }
        }

        public void SendWave(string wave, BattleAnalyticsData data)
        {
            if (!IsComponentsReady()) return;
            SendEvent("battle_completed", new Dictionary<string, object>
            {
                {"TimelineNumber", data.TimelineNumber},
                {"AgeNumber", data.AgeNumber},
                {"BattleNumber", data.BattleNumber},
                {"Wave", wave},
            });
        }

        private void OnStepCompleted(int step)
        {
            var trueStepNumber = step + 1;
            int lastStep = 5;
            if (trueStepNumber == lastStep)
            {
                _appMetricaComp.SendCustomEvent("tutorial", new Dictionary<string, object>() {
                {"step_name", $"{lastStep}_mainTutorFinish" },
            }, true);
                return;
            }
            _appMetricaComp.SendCustomEvent("tutorial", new Dictionary<string, object>() {
                {"step_name", $"{trueStepNumber}_mainTutor" },
            }, true);
        }
    }

}
