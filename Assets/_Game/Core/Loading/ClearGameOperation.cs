using System;
using _Game.Core.Communication;
using _Game.Core.GameState;
using _Game.Core.Scripts;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.GameModes.BattleMode.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class ClearGameOperation : ILoadingOperation
    {
        public string Description => "Clearing..."; 
        
        private readonly IGameModeCleaner _gameModeCleanUp;
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IPersistentDataService _persistentData;
        private readonly IShopPopupProvider _shopPopupProvider;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IAudioService _audioService;
        private readonly IUserStateCommunicator _communicator;

        public ClearGameOperation(
            SceneLoader sceneLoader,
            IGameModeCleaner gameModeCleanUp,
            IWorldCameraService cameraService,
            IGameStateMachine stateMachine,
            IPersistentDataService persistentData,
            ISettingsPopupProvider settingsPopupProvider,
            IShopPopupProvider shopPopupProvider,
            IAudioService audioService,
            IUserStateCommunicator communicator)
        {
            _sceneLoader = sceneLoader;
            _gameModeCleanUp = gameModeCleanUp;
            _cameraService = cameraService;
            _stateMachine = stateMachine;
            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
            _shopPopupProvider = shopPopupProvider;
            _audioService = audioService;
            _communicator = communicator;
        }
        public async UniTask Load(Action<float> onProgress)
        {
            //TODO Delete
            Debug.Log("Clearing game operation");
            
            onProgress?.Invoke(0.2f);
            _gameModeCleanUp.Cleanup();
        
            foreach (var factory in _gameModeCleanUp.Factories)
            {
                await factory.Unload();
            }

            onProgress?.Invoke(0.75f);
        
            var unloadOp = _sceneLoader.UnloadSceneAsync(_gameModeCleanUp.SceneName);
        
            while (unloadOp.isDone == false)
            {
                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
        }
    }
}