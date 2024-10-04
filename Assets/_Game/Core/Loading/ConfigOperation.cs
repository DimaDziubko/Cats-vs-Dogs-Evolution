using System;
using System.Threading.Tasks;
using _Game.Core._Logger;
using _Game.Core.Configs.Providers;
using _Game.Core.Services.UserContainer;
using _Game.Utils._LocalConfigSaver;
using _Game.Utils.Extensions;
using Assets._Game.Core.Loading;
using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Core.Loading
{
    public enum ConfigurationLevel
    {
        Game,
        Timeline
    }
    
    public sealed class ConfigOperation : ILoadingOperation
    {
        public string Description => "Configuration loading...";
        
        private readonly IUserContainer _userContainer;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly IMyLogger _logger;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly ConfigurationLevel _configurationLevel;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public ConfigOperation(
            IUserContainer userContainer,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IMyLogger logger,
            ConfigurationLevel configurationLevel = ConfigurationLevel.Game)
        {
            _configurationLevel = configurationLevel;
            _userContainer = userContainer;
            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            switch (_configurationLevel)
            {
                case ConfigurationLevel.Game:
                    await LoadGameConfig(onProgress);
                    break;
                case ConfigurationLevel.Timeline:
                    LoadTimelineConfig(onProgress);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadTimelineConfig(Action<float> onProgress)
        {
            onProgress?.Invoke(0.0f);
            var config = _localConfigProvider.GetConfig();
            JObject jObject = JObject.Parse(config);
            _userContainer.GameConfig.CurrentTimeline = jObject.ForTimeline(TimelineState.TimelineId);
            onProgress?.Invoke(1.0f);
        }

        private async Task LoadGameConfig(Action<float> onProgress)
        {
            try
            {
                onProgress?.Invoke(0.0f);
                JObject configData = await _remoteConfigProvider.GetConfig();

                onProgress?.Invoke(0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                onProgress?.Invoke(0.2f);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                onProgress?.Invoke(0.3f);

                if (configData != null)
                {
                    _userContainer.GameConfig = configData.ToGameConfig(TimelineState.TimelineId);

                    LocalConfigSaver.SaveConfig(configData.ToString());

                    _remoteConfigProvider.ClearCache();
                }
                else
                {
                    _logger.LogWarning($"Remote config not available");
                    throw new Exception("Remote config not available");
                }

                onProgress?.Invoke(0.5f);
            }
            catch (Exception e)
            {
                _logger.Log($"Catch block with exception {e}");
                
                try
                {
                    string configString = _localConfigProvider.GetConfig();
                    JObject configData = JObject.Parse(configString);
                    _userContainer.GameConfig = configData.ToGameConfig(TimelineState.TimelineId);

                    _logger.Log("Remote config is not available, using local.");
                }
                catch (Exception parseException)
                {
                    _logger.LogError("Failed to parse local config");
                }
            }

            onProgress?.Invoke(1.0f);
        }
    }
}