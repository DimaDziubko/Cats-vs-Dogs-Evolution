using System;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace _Game.Core.Loading
{
    public class InitializationOperation : ILoadingOperation
    {
        public string Description => "Battle initialization...";
        
        private readonly IBattleStateService _battleState;
        private readonly IAgeStateService _ageState;
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IUnitUpgradesService _unitUpgradesService;
        private readonly IAssetProvider _assetProvider;
        private readonly IEvolutionService _evolutionService;

        public InitializationOperation(
            IBattleStateService battleState,
            IAgeStateService ageState,
            IEconomyUpgradesService economyUpgradesService,
            IUnitUpgradesService unitUpgradesService,
            IAssetProvider assetProvider,
            IEvolutionService evolutionService)
        {
            _battleState = battleState;
            _ageState = ageState;
            _economyUpgradesService = economyUpgradesService;
            _unitUpgradesService = unitUpgradesService;
            _assetProvider = assetProvider;
            _evolutionService = evolutionService;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.1f);
            _assetProvider.Init();
            onProgress.Invoke(0.2f);
            await _battleState.Init();
            onProgress.Invoke(0.4f);
            await _ageState.Init();
            onProgress.Invoke(0.6f);
            await _economyUpgradesService.Init();
            onProgress.Invoke(0.8f);
            await _unitUpgradesService.Init();
            onProgress.Invoke(0.9f);
            _evolutionService.Init();
            onProgress.Invoke(1.0f);
        }
    }
}