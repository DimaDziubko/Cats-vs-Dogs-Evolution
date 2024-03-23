using System;
using _Game.Core.GameState;
using _Game.Core.Scripts;
using _Game.Core.Services.Camera;
using _Game.GameModes.BattleMode.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Core.Loading
{
    public sealed class GameLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IGameStateMachine _stateMachine;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IShopPopupProvider _shopPopupProvider;

        public string Description => "Game loading...";
        
        public GameLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IGameStateMachine stateMachine)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _stateMachine = stateMachine;
        }
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var loadOp = _sceneLoader.LoadSceneAsync(Constants.Scenes.BATTLE_MODE,
                LoadSceneMode.Single);
            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            onProgress?.Invoke(0.7f);
            
            Scene scene = _sceneLoader.GetSceneByName(Constants.Scenes.BATTLE_MODE);
            
            var gameMode = scene.GetRoot<BattleMode>();
            
            _cameraService.EnableCamera();
            
            onProgress?.Invoke(0.85f);
            
            gameMode.Init();
            
            onProgress?.Invoke(1.0f);
            
            //_stateMachine.Enter<MenuState>();
        }
    }
}