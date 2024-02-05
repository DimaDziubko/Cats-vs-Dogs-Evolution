using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Providers;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.Utils._LocalConfigSaver;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Core.Loading
{
    public sealed class ConfigOperation : ILoadingOperation
    {
        public string Description => "Configuration loading...";
        
        private readonly IPersistentDataService _persistentData;
        private readonly IAssetProvider _assetProvider;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly ILocalConfigProvider _localConfigProvider;

        public ConfigOperation(
            IPersistentDataService persistentData,
            IAssetProvider assetProvider,
            IRemoteConfigProvider remoteConfigProvider,
            ILocalConfigProvider localConfigProvider)
        {
            _persistentData = persistentData;
            _assetProvider = assetProvider;
            _remoteConfigProvider = remoteConfigProvider;
            _localConfigProvider = localConfigProvider;
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
                    
                    _remoteConfigProvider.CleanCache();
                }
                else
                {
                    throw new Exception("Remote config not available");
                }
                onProgress?.Invoke(0.5f); 
            }
            catch (Exception e)
            {
                Debug.Log("Remote config are not available. Using local config.");
                var localConfigString = _localConfigProvider.GetConfig(); 
                
                if (!string.IsNullOrEmpty(localConfigString))
                {
                    _persistentData.GameConfig = JsonConvert.DeserializeObject<GameConfig>(localConfigString); 
                    onProgress?.Invoke(0.7f); 
                }
                else
                {
                    Debug.Log("No local configuration available.");
                    onProgress?.Invoke(0.8f);
                    return;
                }
            }
            
            onProgress?.Invoke(1.0f); 
        }
        
    }
}