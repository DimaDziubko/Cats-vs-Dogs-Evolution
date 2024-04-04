using System;
using _Game.Core.Ads;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using Cysharp.Threading.Tasks;

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
        private readonly IAdsService _adsService;
        private readonly IBonusRewardService _bonusRewardService;

        public InitializationOperation(
            IBattleStateService battleState,
            IAgeStateService ageState,
            IEconomyUpgradesService economyUpgradesService,
            IUnitUpgradesService unitUpgradesService,
            IAssetProvider assetProvider,
            IEvolutionService evolutionService,
            IAdsService adsService,
            IBonusRewardService bonusRewardService)
        {
            _battleState = battleState;
            _ageState = ageState;
            _economyUpgradesService = economyUpgradesService;
            _unitUpgradesService = unitUpgradesService;
            _assetProvider = assetProvider;
            _evolutionService = evolutionService;
            _adsService = adsService;
            _bonusRewardService = bonusRewardService;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.1f);
            _assetProvider.Init();
            onProgress.Invoke(0.2f);
            await _battleState.Init();
            onProgress.Invoke(0.3f);
            await _ageState.Init();
            onProgress.Invoke(0.4f);
            await _economyUpgradesService.Init();
            onProgress.Invoke(0.5f);
            await _unitUpgradesService.Init();
            onProgress.Invoke(0.6f);
            await _evolutionService.Init();
            onProgress.Invoke(0.7f);
            _adsService.Init();
            onProgress.Invoke(0.8f);
            await _bonusRewardService.Init();
            onProgress.Invoke(1.0f);
        }
    }
}