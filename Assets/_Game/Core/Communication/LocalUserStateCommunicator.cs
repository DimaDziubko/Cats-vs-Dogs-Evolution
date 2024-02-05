using System.IO;
using _Game.Core.UserState;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Communication
{
    public class LocalUserStateCommunicator : IUserStateCommunicator
    {
        private readonly ISaveLoadStrategy _strategy;
        private string SaveFolder => $"{Application.persistentDataPath}/game_saves";
        private string FileName => "userAccountState.json";
        private string Path => $"{SaveFolder}/{FileName}";

        public LocalUserStateCommunicator(ISaveLoadStrategy strategy)
        {
            _strategy = strategy;
            EnsureSaveFolderExists();
        }

        private void EnsureSaveFolderExists()
        {
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }
        }

        public async UniTask<bool> SaveUserState(UserAccountState state)
        {
            return await _strategy.SaveUserState(state, Path);
        }

        public async UniTask<UserAccountState> GetUserState()
        {
            return await _strategy.GetUserState(Path);
        }
    }
}