using System.Collections.Generic;
using _Game.Core.Ads;
using _Game.Core.Loading;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.Battle;
using _Game.Core.Services.BonusReward.Scripts;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._Tutorial.Scripts;

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
        private readonly IBonusRewardService _bonusRewardService;
        private readonly TutorialManager _tutorialManager;

        public InitializationState(
            IGameStateMachine stateMachine,
            IAssetProvider assetProvider,
            IBattleStateService battleState,
            IEconomyUpgradesService economyUpgradesService,
            IAgeStateService ageState, 
            IUnitUpgradesService unitUpgradeService,
            IEvolutionService evolutionService,
            IAdsService adsService,
            IBonusRewardService bonusRewardService,
            TutorialManager tutorialManager)
        {
            _stateMachine = stateMachine;
            _assetProvider = assetProvider;
            _battleState = battleState;
            _economyUpgradesService = economyUpgradesService;
            _ageState = ageState;
            _unitUpgradeService = unitUpgradeService;
            _evolutionService = evolutionService;
            _adsService = adsService;
            _bonusRewardService = bonusRewardService;
            _tutorialManager = tutorialManager;
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
                    _bonusRewardService,
                    _tutorialManager));
            
            _stateMachine.Enter<GameLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}