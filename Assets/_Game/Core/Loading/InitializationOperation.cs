using System;
using _Game.Core._GameInitializer;
using Assets._Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class InitializationOperation : ILoadingOperation
    {
        public string Description => "Initialization...";
        
        private readonly IGameInitializer _gameInitializer;

        public InitializationOperation(IGameInitializer gameInitializer) => 
            _gameInitializer = gameInitializer;

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.05f);
            await _gameInitializer.InitAsync();
            onProgress.Invoke(0.5f);
            _gameInitializer.Init();
            onProgress.Invoke(1.0f);
        }
    }
}