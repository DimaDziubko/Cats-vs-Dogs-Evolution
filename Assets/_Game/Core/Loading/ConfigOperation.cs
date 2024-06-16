using System;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Providers;
using _Game.Core.Services.PersistentData;
using _Game.Utils._LocalConfigSaver;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace _Game.Core.Loading
{
    public sealed class ConfigOperation : ILoadingOperation
    {
        public string Description => "Configuration loading...";
        
        private readonly IUserContainer _persistentData;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly ILocalConfigProvider _localConfigProvider;
        private readonly IMyLogger _logger;

        public ConfigOperation(
            IUserContainer persistentData,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;
            _logger = logger;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            try
            {
                onProgress?.Invoke(0.0f); 
                var configData = await _remoteConfigProvider.GetConfig();
                
                onProgress?.Invoke(0.1f); 
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                onProgress?.Invoke(0.2f); 
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                onProgress?.Invoke(0.3f); 
                
                if (configData != null)
                {
                    _persistentData.GameConfig = configData.ToGameConfig();
                    
                    var configString = _persistentData.GameConfig.ToJsonString();
                    
                    LocalConfigSaver.SaveConfig(configString); 
                    
                    _remoteConfigProvider.ClearCache();
                }
                else
                {
                    throw new Exception("Remote config not available");
                }
                onProgress?.Invoke(0.5f); 
            }
            catch (Exception e)
            {
                _logger.Log("Remote config are not available. Using local config.");
                var localConfigString = _localConfigProvider.GetConfig(); 
                
                if (!string.IsNullOrEmpty(localConfigString))
                {
                    _persistentData.GameConfig = JsonConvert.DeserializeObject<GameConfig>(localConfigString); 
                    onProgress?.Invoke(0.7f); 
                }
                else
                {
                    _logger.Log("No local configuration available.");
                    onProgress?.Invoke(0.8f);
                    return;
                }
            }
            
            onProgress?.Invoke(1.0f); 
        }
        
    }
}