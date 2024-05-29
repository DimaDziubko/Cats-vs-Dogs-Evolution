using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameSaver;
using _Game.Core.Ads;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Battle;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI.Pin.Scripts;
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
        private readonly IFoodBoostService _foodBoostService;
        private readonly ITutorialManager _tutorialManager;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameSaver _gameSaver;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IBattleSpeedService _battleSpeed;
        private readonly IAnalyticsService _analytics;
        private readonly IDTDAnalyticsService _dtdAnalytics;

        public InitializationOperation(
            IBattleStateService battleState,
            IAgeStateService ageState,
            IEconomyUpgradesService economyUpgradesService,
            IUnitUpgradesService unitUpgradesService,
            IAssetProvider assetProvider,
            IEvolutionService evolutionService,
            IAdsService adsService,
            IFoodBoostService foodBoostService,
            ITutorialManager tutorialManager,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameSaver gameSaver,
            IUpgradesAvailabilityChecker upgradesChecker,
            IBattleSpeedService battleSpeed,
            IAnalyticsService analytics,
            IDTDAnalyticsService dtdAnalytics)
        {
            _battleState = battleState;
            _ageState = ageState;
            _economyUpgradesService = economyUpgradesService;
            _unitUpgradesService = unitUpgradesService;
            _assetProvider = assetProvider;
            _evolutionService = evolutionService;
            _adsService = adsService;
            _foodBoostService = foodBoostService;
            _tutorialManager = tutorialManager;
            _featureUnlockSystem = featureUnlockSystem;
            _gameSaver = gameSaver;
            _upgradesChecker = upgradesChecker;
            _battleSpeed = battleSpeed;
            _analytics = analytics;
            _dtdAnalytics = dtdAnalytics;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress.Invoke(0.05f);
            _adsService.Init();
            onProgress.Invoke(0.1f);
            await _analytics.Init();
            onProgress.Invoke(0.2f);
            _dtdAnalytics.Init();
            onProgress.Invoke(0.15f);
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
            _battleSpeed.Init();
            onProgress.Invoke(0.85f);
            await _foodBoostService.Init();
            onProgress.Invoke(0.9f);
            _tutorialManager.Init();
            onProgress.Invoke(0.95f);
            _featureUnlockSystem.Init();
            onProgress.Invoke(0.97f);
            _gameSaver.Init();
            onProgress.Invoke(0.98f);
            _upgradesChecker.Init();
            onProgress.Invoke(1.0f);
        }
    }
}