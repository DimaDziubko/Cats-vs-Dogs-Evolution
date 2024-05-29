using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameSaver;
using _Game.Core.Ads;
using _Game.Core.Loading;
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

namespace _Game.Core.GameState
{
    public class InitializationState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IAssetProvider _assetProvider;
        private readonly IBattleStateService _battleState;
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IAgeStateService _ageState;
        private readonly IUnitUpgradesService _unitUpgradeService;
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

        public InitializationState(
            IGameStateMachine stateMachine,
            IAssetProvider assetProvider,
            IBattleStateService battleState,
            IEconomyUpgradesService economyUpgradesService,
            IAgeStateService ageState, 
            IUnitUpgradesService unitUpgradeService,
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
            _stateMachine = stateMachine;
            _assetProvider = assetProvider;
            _battleState = battleState;
            _economyUpgradesService = economyUpgradesService;
            _ageState = ageState;
            _unitUpgradeService = unitUpgradeService;
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
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new InitializationOperation(
                    _battleState,
                    _ageState, 
                    _economyUpgradesService, 
                    _unitUpgradeService,
                    _assetProvider,
                    _evolutionService,
                    _adsService,
                    _foodBoostService,
                    _tutorialManager,
                    _featureUnlockSystem,
                    _gameSaver,
                    _upgradesChecker,
                    _battleSpeed,
                    _analytics,
                    _dtdAnalytics));
            
            _stateMachine.Enter<GameLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}