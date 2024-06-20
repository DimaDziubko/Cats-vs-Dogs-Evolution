using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Battle.Scripts;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Installations;
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

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        
        public AnalyticsService(
            IMyLogger logger, 
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _logger = logger;
            _userContainer = userContainer;
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
                await FetchUniqueIDAsync();
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
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.OpenedUnit -= OnUnitOpened;
            RaceState.Changed -= OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleChanged;
            _app?.Dispose();
        }

        private void OnCompletedBattleChanged()
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
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
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            if (RaceState.Counter == 1)
            {
                SendEvent($"race_selected_{RaceState.CurrentRace.ToString()}");
                return;
            }
            
            SendEvent($"race_changed_{RaceState.CurrentRace.ToString()}");
        }

        public void OnBattleStarted(BattleAnalyticsData battleAnalyticsData)
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"battle_started_{battleAnalyticsData.BattleNumber}");
        }

        private bool IsComponentsReady() => 
            _isFirebaseInitialized || _userContainer != null;

        private void OnUnitOpened(UnitType type)
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"unit_opened_{type}");
        }

        private void OnNextBattleOpened()
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"battle_completed_{TimelineState.MaxBattle}");
            // SendEvent("battle_completed", new Dictionary<string, object>
            // {
            //     {"TimelineNumber", TimelineState.TimelineId + 1},
            //     {"AgeNumber", TimelineState.AgeId + 1},
            //     {"BattleNumber", TimelineState.MaxBattle}
            // });
        }

        private void OnNextAgeOpened()
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"evolution_completed_{TimelineState.AgeId}_timeline_{TimelineState.TimelineId + 1}");
        }

        private async UniTask FetchUniqueIDAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            try
            {
                var token = await FirebaseInstallations.DefaultInstance.GetTokenAsync(forceRefresh: true);
                UniqueID = token;
                _logger.Log($"Firebase Unique ID: {UniqueID}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Firebase Unique ID: {ex.Message}");
                
                UniqueID = SystemInfo.deviceUniqueIdentifier;
                if (string.IsNullOrEmpty(UniqueID))
                {
                    UniqueID = Guid.NewGuid().ToString();
                }
                _logger.Log($"Generated fallback Unique ID: {UniqueID}");
            }
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
    }

}
