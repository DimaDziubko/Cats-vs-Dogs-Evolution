using System.Collections.Generic;
using _Game.Core.AssetManagement;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class LoadingScreenProvider : LocalAssetLoader, ILoadingScreenProvider
    {
        public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations)
        {
            var loadingScreen = await Load<LoadingScreen>(AssetsConstants.LOADING_SCREEN);
            await loadingScreen.Load(loadingOperations);
            Unload();
        }
    }
}