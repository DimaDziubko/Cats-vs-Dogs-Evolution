using System;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.StaticData;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class AssetLoadingOperation : ILoadingOperation
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IGameConfigController _gameConfigController;
        public string Description => "Loading assets";

        public AssetLoadingOperation(
            IAssetProvider assetProvider, 
            IGameConfigController gameConfigController)
        {
            _assetProvider = assetProvider;
            _gameConfigController = gameConfigController;
        }

        public async UniTask Load(Action<float> onProgress)
        {
  
        }
    }
}