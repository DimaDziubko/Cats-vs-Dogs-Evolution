using System;
using _Game.UI._MainMenu.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class MainMenuLoadingOperation : ILoadingOperation
    {
        private readonly IMainMenuProvider _provider;
        public string Description => "Loading menu...";

        public MainMenuLoadingOperation(IMainMenuProvider provider)
        {
            _provider = provider;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0);
            await _provider.Load();
            onProgress?.Invoke(1);
        }
    }
}