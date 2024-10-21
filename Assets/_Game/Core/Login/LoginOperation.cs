using System;
using _Game.Core.Communication;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using Assets._Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Login
{
    public class LoginOperation : ILoadingOperation
    {
        public string Description => "Login...";
        
        private Action<float> _onProgress;
        private readonly IUserContainer _userContainer;
        private readonly IUserStateCommunicator _communicator;
        private readonly IRandomService _random;

        public LoginOperation(
            IUserContainer userContainer,
            IUserStateCommunicator communicator,
            IRandomService random)
        {
            _userContainer = userContainer;
            _communicator = communicator;
            _random = random;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            _onProgress = onProgress;
            _onProgress?.Invoke(0.3f);

            _userContainer.State = await GetAccountState();

            _onProgress?.Invoke(1f);
        }
        
        private async UniTask<UserAccountState> GetAccountState()
        {
            UserAccountState result = await _communicator.GetUserState();
            
            _onProgress?.Invoke(0.6f);

            if (result == null || result.IsValid() == false)
            {
                result = UserAccountState.GetInitial(_random);

                await _communicator.SaveUserState(result);
            }

            return result;
        }
    }
    
}