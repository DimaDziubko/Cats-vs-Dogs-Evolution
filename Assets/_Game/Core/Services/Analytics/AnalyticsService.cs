using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
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
        private readonly IPersistentDataService _persistentData;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public AnalyticsService(IMyLogger logger, IPersistentDataService persistentData)
        {
            _logger = logger;
            _persistentData = persistentData;
        }

        public async UniTask Init()
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
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            _app?.Dispose();
        }

        public void OnBattleStarted(BattleAnalyticsData battleAnalyticsData)
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"battle_started_{battleAnalyticsData.BattleNumber}");
            SendEvent("battle_started", new Dictionary<string, object>
            {
                {"timeline", battleAnalyticsData.TimelineNumber},
                {"age", battleAnalyticsData.AgeNumber},
                {"battle", battleAnalyticsData.BattleNumber}
            });
        }

        private bool IsComponentsReady()
        {
            return _isFirebaseInitialized || _persistentData != null;
        }

        private void OnNextBattleOpened()
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"battle_completed_{TimelineState.MaxBattle}");
            SendEvent("battle_completed", new Dictionary<string, object>
            {
                {"TimelineNumber", TimelineState.TimelineId + 1},
                {"AgeNumber", TimelineState.AgeId + 1},
                {"BattleNumber", TimelineState.MaxBattle}
            });
        }

        private void OnNextAgeOpened()
        {
            if (!IsComponentsReady())
            {
                _logger.Log("Firebase is not initialized or persistent data is null.");
                return;
            }
            
            SendEvent($"evolution_completed_{TimelineState.AgeId}");
            SendEvent("evolution_completed", new Dictionary<string, object>
            {
                {"TimelineNumber", TimelineState.TimelineId + 1},
                {"AgeNumber", TimelineState.AgeId}
            });
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

        private void SendEvent(string eventName)
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
