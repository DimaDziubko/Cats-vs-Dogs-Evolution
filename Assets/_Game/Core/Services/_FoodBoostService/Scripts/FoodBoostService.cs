using System;
using _Game.Common;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Hud;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core.UserState;
using CAS;
using UnityEngine;

namespace _Game.Core.Services._FoodBoostService.Scripts
{
    public class FoodBoostService : IFoodBoostService, IDisposable, IFoodConsumer
    {
        public event Action<int, bool> ChangeFood;
        public event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;

        private readonly IUserContainer _userContainer;
        private readonly IEconomyConfigRepository _configRepository;
        private readonly IAdsService _adsService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly ICommonItemsConfigRepository _commonConfig;

        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IUpgradeItemsReadonly UpgradeItems => _generalDataPool.AgeDynamicData.UpgradeItems;
        private IFoodBoostStateReadonly FoodBoostState => _userContainer.State.FoodBoost;

        private readonly FoodBoostBtnModel _foodBoostBtnModel = new FoodBoostBtnModel();

        public FoodBoostService(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IAdsService adsService,
            IFeatureUnlockSystem featureUnlockSystem,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _configRepository = configRepositoryFacade.EconomyConfigRepository;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _logger = logger;
            _adsService = adsService;
            _featureUnlockSystem = featureUnlockSystem;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        void IFoodListener.OnFoodBalanceChanged(int value) { }

        void IFoodListener.OnFoodGenerated() { }

        private void Init()
        {
            UpdateFoodBoost();
            
            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            _adsService.VideoLoaded += OnRewardVideoLoaded;

        }

        void IDisposable.Dispose()
        {
            FoodBoostState.FoodBoostChanged -= OnFoodBoostChanged;
            _adsService.VideoLoaded -= OnRewardVideoLoaded;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnRewardVideoLoaded(AdType type)
        {
            if(type == AdType.Rewarded)
                UpdateFoodBoostBtnModel();
        }

        public void OnFoodBoostShown()
        {
            UpdateFoodBoost();
            UpdateFoodBoostBtnModel();
        }

        public void OnFoodBoostBtnClicked() => 
            _adsService.ShowRewardedVideo(OnFoodBoostRewardedVideoComplete, Placement.Food);

        private void OnFoodBoostChanged() => 
            UpdateFoodBoostBtnModel();

        private void UpdateFoodBoost()
        {
            var foodBoostConfig = _configRepository.GetFoodBoostConfig();

            DateTime now = DateTime.UtcNow;

            TimeSpan timeSinceLastBoost = now - FoodBoostState.LastDailyFoodBoost;
            int recoverableBoosts = (int)(timeSinceLastBoost.TotalMinutes / foodBoostConfig.RecoverTimeMinutes);

            if (recoverableBoosts > 0)
            {
                int lackingBoosts = foodBoostConfig.DailyFoodBoostCount - FoodBoostState.DailyFoodBoostCount;
                int boostsToAdd = Mathf.Min(recoverableBoosts, lackingBoosts);
                
                DateTime newLastDailyFoodBoost = FoodBoostState.LastDailyFoodBoost.AddMinutes(boostsToAdd * foodBoostConfig.RecoverTimeMinutes);
                _userContainer.FoodBoostStateHandler.RecoverFoodBoost(
                    boostsToAdd,
                    newLastDailyFoodBoost);
                
                UpdateFoodBoostBtnModel();
            }
        }

        private void OnFoodBoostRewardedVideoComplete()
        {
            var foodBoostConfig = _configRepository.GetFoodBoostConfig();
            if (FoodBoostState.DailyFoodBoostCount == foodBoostConfig.DailyFoodBoostCount)
            {
                _userContainer.FoodBoostStateHandler.SpendFoodBoost(DateTime.UtcNow);
            }
            else
            {
                _userContainer.FoodBoostStateHandler.SpendFoodBoost(FoodBoostState.LastDailyFoodBoost);
            }
            
            ChangeFood?.Invoke(_foodBoostBtnModel.FoodAmount, true);
        }

        private void UpdateFoodBoostBtnModel()
        {
            FoodBoostConfig foodBoostConfig = _configRepository.GetFoodBoostConfig();
            
            _foodBoostBtnModel.FoodIcon = _commonConfig.ForFoodIcon(RaceState.CurrentRace);
            _foodBoostBtnModel.FoodAmount = (int)(UpgradeItems.GetItemData(UpgradeItemType.FoodProduction).Amount 
                                                  * foodBoostConfig.FoodBoostCoefficient);
            
            _foodBoostBtnModel.IsAvailable = 
                FoodBoostState.DailyFoodBoostCount > 0 && _featureUnlockSystem.IsFeatureUnlocked(Feature.FoodBoost);
            
            _foodBoostBtnModel.IsInteractable = _adsService.IsAdReady(AdType.Rewarded);

            FoodBoostBtnModelChanged?.Invoke(_foodBoostBtnModel);
        }
    }
}