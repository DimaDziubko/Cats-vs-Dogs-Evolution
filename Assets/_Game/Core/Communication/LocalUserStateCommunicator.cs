using System;
using System.IO;
using _Game.Core.UserState;
using _Game.Gameplay.Common.Scripts;
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
            var result =  await _strategy.GetUserState(Path);
            
            // result.RaceState ??= new RaceState()
            // {
            //     CurrentRace = Race.None
            // };
            //
            // result.FoodBoost ??= new FoodBoostState()
            // {
            //     DailyFoodBoostCount = 2,
            //     LastDailyFoodBoost = DateTime.UtcNow
            // };
            
            return result;
        }
    }
}