using System;
using System.Threading.Tasks;
using _Game.Utils.Extensions;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace _Game.Core.Configs.Providers
{
    public class RemoteConfigProvider : IRemoteConfigProvider
    {
        private JObject _cachedConfig;
        private bool _isConfigLoaded;

        public async Task<JObject> GetConfig()
        {
            if (!_isConfigLoaded)
            {
                await LoadConfig();
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
            
            return _cachedConfig;
        }

        public void CleanCache()
        {
            _cachedConfig = null;
            _isConfigLoaded = false;
        }

        private async Task LoadConfig()
        {
            Debug.Log("Fetching data...");
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                ProcessFetchedConfig();
            }
            catch (Exception e)
            {
                Debug.Log("Error fetching remote config: " + e.Message);
            }
        }

        private void ProcessFetchedConfig()
        {
            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError($"Fetch was unsuccessful\nLastFetchStatus: {info.LastFetchStatus}");
                return;
            }
            
            remoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}");
                        _cachedConfig = ParseConfig(remoteConfig);
                        _isConfigLoaded = true;
                    });
        }

        private JObject ParseConfig(FirebaseRemoteConfig remoteConfig)
        {
            var configString = remoteConfig.GetValue("GameConfig").StringValue;

            configString = configString.FixJsonArrays();
            
            var configJsonData = JObject.Parse(configString);
            
            return configJsonData;
        }
    }

    public interface IRemoteConfigProvider
    {
        Task<JObject> GetConfig();
        void CleanCache();
    }
}