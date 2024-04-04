using System;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class ChangingRaceOperation : ILoadingOperation
    {
        public string Description => "Changing race...";

        private readonly IAgeStateService _ageStateService;
        private readonly IBattleStateService _battleStateService;

        public ChangingRaceOperation(
            IAgeStateService ageStateService,
            IBattleStateService battleStateService)
        {
            _ageStateService = ageStateService;
            _battleStateService = battleStateService;
        }
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.2f);
            await _ageStateService.ChangeRace();
            onProgress.Invoke(0.5f);
            await _battleStateService.ChangeRace();
            onProgress.Invoke(1.0f);
        }
    }
}