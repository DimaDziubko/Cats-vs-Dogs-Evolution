﻿using System;
using System.Collections.Generic;
using _Game.Core.AssetManagement;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class LoadingScreenProvider : LocalAssetLoader, ILoadingScreenProvider
    {
        public event Action LoadingCompleted;

        public async UniTask LoadAndDestroy(ILoadingOperation loadingOperation)
        {
            var operations = new Queue<ILoadingOperation>();
            operations.Enqueue(loadingOperation);
            await LoadAndDestroy(operations);
        }

        public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations)
        {
            var loadingScreen = await Load<LoadingScreen>(AssetsConstants.LOADING_SCREEN);
            await loadingScreen.Load(loadingOperations);
            Unload();
            LoadingCompleted?.Invoke();
        }
    }
}