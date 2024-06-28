using System.IO;
using _Game.Core.UserState;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Core.Communication
{
    public class JsonSaveLoadStrategy : ISaveLoadStrategy
    {
        private readonly StateMigrationManager _migrationManager = new StateMigrationManager();
        
        public async UniTask<bool> SaveUserState(UserAccountState state, string path)
        {
            string json = JsonConvert.SerializeObject(state);
            await File.WriteAllTextAsync(path, json);
            return true;
        }

        public async UniTask<UserAccountState> GetUserState(string path)
        {
            if (!File.Exists(path)) return null;

            string json = await File.ReadAllTextAsync(path);

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate
            };

            var state = JsonConvert.DeserializeObject<UserAccountState>(json, settings);

            if (state != null && state.Version != Application.version)
            {
                _migrationManager.Migrate(ref state);
            }
            
            return state;
        }
    }
}