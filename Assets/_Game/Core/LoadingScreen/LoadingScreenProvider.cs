using System;
using System.Collections.Generic;
using _Game.Core.AssetManagement;
using Assets._Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.LoadingScreen
{
    public class LoadingScreenProvider : LocalAssetLoader, ILoadingScreenProvider
    {
        public event Action LoadingCompleted;

        public async UniTask LoadAndDestroy(ILoadingOperation loadingOperation, LoadingScreenType type)
        {
            var operations = new Queue<ILoadingOperation>();
            operations.Enqueue(loadingOperation);
            await LoadAndDestroy(operations, type);
        }

        public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations, LoadingScreenType type)
        {
            var loadingScreen = await Load<LoadingScreen>(AssetsConstants.LOADING_SCREEN);
            await loadingScreen.Load(loadingOperations, type);
            LoadingCompleted?.Invoke();
            Unload();
        }
    }
}