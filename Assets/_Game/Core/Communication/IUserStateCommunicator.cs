using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.Communication
{
    public interface IUserStateCommunicator
    {
        UniTask<bool> SaveUserState(UserAccountState state);
        UniTask<UserAccountState> GetUserState();
    }
}