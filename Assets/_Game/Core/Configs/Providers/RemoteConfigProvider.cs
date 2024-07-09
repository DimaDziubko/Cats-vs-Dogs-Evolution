using System;
using System.Threading.Tasks;
using Assets._Game.Core._GameMode;
using Assets._Game.Core._Logger;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Configs.Providers
{
    public class RemoteConfigProvider : IRemoteConfigProvider
    {
        private readonly IMyLogger _logger;
        
        private JObject _cachedConfig;
        private bool _isConfigLoaded;

        public RemoteConfigProvider(IMyLogger logger)
        {
            _logger = logger;
        }
        
        public async Task<JObject> GetConfig()
        {
            if (!_isConfigLoaded)
            {
                await LoadConfig();
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
            
            return _cachedConfig;
        }

        public void ClearCache()
        {
            _cachedConfig = null;
            _isConfigLoaded = false;
        }

        private async Task LoadConfig()
        {
            _logger.Log("Fetching data...");
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                ProcessFetchedConfig();
            }
            catch (Exception e)
            {
                _logger.Log("Error fetching remote config: " + e.Message);
            }
        }

        private void ProcessFetchedConfig()
        {
            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                _logger.LogError($"Fetch was unsuccessful\nLastFetchStatus: {info.LastFetchStatus}");
                return;
            }
            
            remoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        _logger.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}");
                        _cachedConfig = ParseConfig(remoteConfig);
                        _isConfigLoaded = true;
                    });
        }

        private JObject ParseConfig(FirebaseRemoteConfig remoteConfig)
        {
            string configString; 
            if (GameMode.I.TestMode)
            {
                configString = remoteConfig.GetValue("TestConfig").StringValue;
                _logger.Log($"TEST CONFIGS loaded");
            }
            else
            {
                configString = remoteConfig.GetValue("GameConfig").StringValue;
                _logger.Log($"PROD CONFIGS loaded");
            }
            
            JObject configJsonData = JObject.Parse(configString);
            
            return configJsonData;
        }
    }

    public interface IRemoteConfigProvider
    {
        Task<JObject> GetConfig();
        void ClearCache();
    }
}