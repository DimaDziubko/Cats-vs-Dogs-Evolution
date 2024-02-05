using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class _RemoteConfigProvider : MonoBehaviour
    {
        [SerializeField] private ConfigHolder _configHolder;
        [SerializeField] private ConfigManager _configManager;
        
        private void Awake()
        {
            CheckRemoteConfigValues();
        }

        public Task CheckRemoteConfigValues()
        {
            Debug.Log("Fetching data...");
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        private void FetchComplete(Task fetchTask)
        {
            if (!fetchTask.IsCompleted)
            {
                Debug.LogError("Retrieval hasn't finished");
                return;
            }

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n {nameof(info.LastFetchStatus)}: " +
                               $"{info.LastFetchStatus}");
                return;
            }
            
            //Fetch successful. Parameter values must be activated to use.
            remoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}");

                        _configHolder.GameConfig = _configManager.ReadConfig(remoteConfig);
                        print("Total values: " + remoteConfig.AllValues.Count);
                    });
        }
    }
}
