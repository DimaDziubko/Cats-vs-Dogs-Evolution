using System.IO;
using _Game.Core.UserState;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace _Game.Core.Communication
{
    public class JsonSaveLoadStrategy : ISaveLoadStrategy
    {
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
            return JsonConvert.DeserializeObject<UserAccountState>(json);
        }
    }
}