using System;
using _Game.Core.Services.Audio;
using _Game.GameModes._BattleMode.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class ClearGameOperation : ILoadingOperation
    {
        public string Description => "Clearing...";
        
        private readonly IGameModeCleaner _gameCleanUp;
        private readonly ISoundService _soundService;

        public ClearGameOperation(
            IGameModeCleaner gameCleanUp,
            ISoundService soundService)
        {
            _gameCleanUp = gameCleanUp;
            _soundService = soundService;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.2f);
            _gameCleanUp.Cleanup();
            _soundService.Cleanup();
            
            foreach (var factory in _gameCleanUp.Factories)
            {
                factory.Cleanup();
                await factory.Unload();
            }
            
            onProgress?.Invoke(0.5f);
            
            _gameCleanUp.ResetGame();
            
            onProgress?.Invoke(1f);
        }
    }
}