using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._Hud;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core.Services._FoodBoostService.Scripts
{
    public class FoodBoostService : IFoodBoostService, IDisposable
    {
        public event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        public event Action<int> FoodBoost;

        private readonly IUserContainer _userContainer;
        private readonly IEconomyConfigRepository _configRepository;
        private readonly IMyLogger _logger;
        private readonly IAdsService _adsService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;

        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IUpgradeItemsReadonly UpgradeItems => _generalDataPool.AgeDynamicData.UpgradeItems;
        private IFoodBoostStateReadonly FoodBoostState => _userContainer.State.FoodBoost;

        private readonly FoodBoostBtnModel _foodBoostBtnModel = new FoodBoostBtnModel();

        public FoodBoostService(
            IUserContainer userContainer,
            IEconomyConfigRepository configRepository,
            IMyLogger logger,
            IAdsService adsService,
            IFeatureUnlockSystem featureUnlockSystem,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _configRepository = configRepository;
            _logger = logger;
            _adsService = adsService;
            _featureUnlockSystem = featureUnlockSystem;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            UpdateFoodBoost();
            
            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;

        }

        void IDisposable.Dispose()
        {
            FoodBoostState.FoodBoostChanged -= OnFoodBoostChanged;
            _adsService.RewardedVideoLoaded -= OnRewardVideoLoaded;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnRewardVideoLoaded() => 
            UpdateFoodBoostBtnModel();

        public void OnFoodBoostShown()
        {
            UpdateFoodBoost();
            UpdateFoodBoostBtnModel();
        }

        public void OnFoodBoostBtnClicked() => 
            _adsService.ShowRewardedVideo(OnFoodBoostRewardedVideoComplete, RewardType.Food);

        private void OnFoodBoostChanged() => 
            UpdateFoodBoostBtnModel();

        private void UpdateFoodBoost()
        {
            var foodBoostConfig = _configRepository.GetFoodBoostConfig();

            DateTime now = DateTime.UtcNow;
            TimeSpan recoverTimeSpan = TimeSpan.FromMinutes(foodBoostConfig.RecoverTimeMinutes);

            if (now - FoodBoostState.LastDailyFoodBoost >= recoverTimeSpan)
            {
                _userContainer.RecoverFoodBoost(foodBoostConfig.DailyFoodBoostCount);
                UpdateFoodBoostBtnModel();
            }
        }
        
        private void OnFoodBoostRewardedVideoComplete()
        {
            _userContainer.SpendFoodBoost();
            FoodBoost?.Invoke(_foodBoostBtnModel.FoodAmount);
        }
        
        private void UpdateFoodBoostBtnModel()
        {
            FoodBoostConfig foodBoostConfig = _configRepository.GetFoodBoostConfig();
            
            _foodBoostBtnModel.FoodIcon = _generalDataPool.AgeStaticData.ForFoodIcon(RaceState.CurrentRace);
            _foodBoostBtnModel.FoodAmount = (int)(UpgradeItems.GetItemData(UpgradeItemType.FoodProduction).Amount 
                                                  * foodBoostConfig.FoodBoostCoefficient);
            
            _foodBoostBtnModel.IsAvailable = 
                FoodBoostState.DailyFoodBoostCount > 0 && _featureUnlockSystem.IsFeatureUnlocked(Feature.FoodBoost);
            
            _foodBoostBtnModel.IsInteractable = _adsService.IsRewardedVideoReady;

            FoodBoostBtnModelChanged?.Invoke(_foodBoostBtnModel);
        }
    }
}