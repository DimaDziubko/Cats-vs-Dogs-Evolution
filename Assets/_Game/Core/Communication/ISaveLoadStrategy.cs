using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.Communication
{
    public interface ISaveLoadStrategy
    {
        UniTask<bool> SaveUserState(UserAccountState state, string path);
        UniTask<UserAccountState> GetUserState(string path);
    }
}