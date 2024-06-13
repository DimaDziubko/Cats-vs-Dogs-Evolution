using System;
using _Game.Core._SceneLoader;
using _Game.Core.Services.Camera;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Game.Core.Loading
{
    public sealed class GameLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IShopPopupProvider _shopPopupProvider;

        public string Description => "Game loading...";
        
        public GameLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
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

            _cameraService.EnableMainCamera();
            onProgress?.Invoke(1.0f);
        }
    }
}