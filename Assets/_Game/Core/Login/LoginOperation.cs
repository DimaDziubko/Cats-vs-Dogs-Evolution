using System;
using _Game.Core.Communication;
using _Game.Core.Loading;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Random;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Login
{
    public class LoginOperation : ILoadingOperation
    {
        public string Description => "Login...";
        
        private Action<float> _onProgress;
        private readonly IPersistentDataService _persistentData;
        private readonly IUserStateCommunicator _communicator;
        private readonly IAssetProvider _staticData;
        private readonly IRandomService _random;

        public LoginOperation(
            IPersistentDataService persistentData,
            IUserStateCommunicator communicator,
            IAssetProvider staticData,
            IRandomService random)
        {
            _persistentData = persistentData;
            _communicator = communicator;
            _staticData = staticData;
            _random = random;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            _onProgress = onProgress;
            _onProgress?.Invoke(0.3f);

            _persistentData.State = await GetAccountState();

            _onProgress?.Invoke(1f);
        }
        
        private async UniTask<UserAccountState> GetAccountState()
        {
            var result = await _communicator.GetUserState();
            _onProgress?.Invoke(0.6f);

            if (result == null || result.IsValid() == false)
            {
                result = UserAccountState.GetInitial(_random, _persistentData.GameConfig);

                await _communicator.SaveUserState(result);
            }

            return result;
        }
    }
    
}