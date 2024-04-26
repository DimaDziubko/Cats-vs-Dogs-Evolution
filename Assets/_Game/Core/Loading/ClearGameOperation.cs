using System;
using _Game.GameModes._BattleMode.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class ClearGameOperation : ILoadingOperation
    {
        public string Description => "Clearing...";
        
        private readonly IGameModeCleaner _gameCleanUp;

        public ClearGameOperation(IGameModeCleaner gameCleanUp)
        {
            _gameCleanUp = gameCleanUp;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            //TODO Delete
            Debug.Log("Clearing game operation");
            
            onProgress?.Invoke(0.2f);
            _gameCleanUp.Cleanup();
        
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