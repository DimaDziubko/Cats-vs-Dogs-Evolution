using System;
using System.Threading;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.UI._Hud;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.BonusReward.Scripts
{
    public class BonusRewardService : IBonusRewardService
    {
        public event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        public event Action<int> FoodBoost;
         
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _configController;
        private readonly IAssetProvider _assetProvider;
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IMyLogger _logger;
        private readonly IAdsService _adsService;
        private readonly IUserStateCommunicator _communicator;

        private IFoodBoostStateReadonly FoodBoostState => _persistentData.State.FoodBoost;

        private readonly FoodBoostBtnModel _foodBoostBtnModel = new FoodBoostBtnModel();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public BonusRewardService(
            IPersistentDataService persistentData,
            IGameConfigController configController,
            IAssetProvider assetProvider,
            IEconomyUpgradesService economyUpgradesService,
            IMyLogger logger,
            IAdsService adsService,
            IUserStateCommunicator communicator)
        {
            _persistentData = persistentData;
            _configController = configController;
            _assetProvider = assetProvider;
            _economyUpgradesService = economyUpgradesService;
            _logger = logger;
            _adsService = adsService;
            _communicator = communicator;
        }

        public async UniTask Init()
        {
            UpdateBonuses();
            await PrepareBonuses();

            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;

        }

        private void OnRewardVideoLoaded() => 
            UpdateFoodBoostBtnModel();

        public void OnHudOpened()
        {
            UpdateFoodBoost();
            UpdateFoodBoostBtnModel();
        }

        public void OnFoodBoostBtnClicked()
        {
            _persistentData.SpendFoodBoost();
            SaveGame();
            _adsService.ShowRewardedVideo(OnRewardedVideoComplete);
        }

        private void OnFoodBoostChanged() => 
            UpdateFoodBoostBtnModel();

        private void UpdateBonuses() => 
            UpdateFoodBoost();

        private void UpdateFoodBoost()
        {
            var foodBoostConfig = _configController.GetFoodBoostConfig();

            DateTime now = DateTime.UtcNow;
            TimeSpan recoverTimeSpan = TimeSpan.FromMinutes(foodBoostConfig.RecoverTimeMinutes);

            if (now - FoodBoostState.LastDailyFoodBoost >= recoverTimeSpan)
            {
                _persistentData.RecoverFoodBoost(foodBoostConfig.DailyFoodBoostCount);
                UpdateFoodBoostBtnModel();
            }
        }

        private async UniTask PrepareBonuses()
        {
            var ct = _cts.Token;
            try
            {
                 
                await PrepareFoodBoostBtnModel(ct);
                
                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }

        private void SaveGame()
        {
            _communicator.SaveUserState(_persistentData.State);
        }

        private void OnRewardedVideoComplete()
        {
            FoodBoost?.Invoke(_foodBoostBtnModel.FoodAmount);
        }

        private async UniTask PrepareFoodBoostBtnModel(CancellationToken ct)
        {
            string foodIconKey = _configController.GetFoodIconKey();
            ct.ThrowIfCancellationRequested();
            
            _foodBoostBtnModel.FoodIcon = await _assetProvider.Load<Sprite>(foodIconKey);
            UpdateFoodBoostBtnModel();
        }

        private void UpdateFoodBoostBtnModel()
        {
            var foodBoostConfig = _configController.GetFoodBoostConfig();
            
            _foodBoostBtnModel.FoodAmount = (int)(_economyUpgradesService.GetFoodProductionSpeed() * foodBoostConfig.FoodBoostCoefficient);
            
            _foodBoostBtnModel.IsAvailable = 
                FoodBoostState.DailyFoodBoostCount > 0;
            
            _foodBoostBtnModel.IsInteractable = _adsService.IsRewardedVideoReady;

            FoodBoostBtnModelChanged?.Invoke(_foodBoostBtnModel);
        }
        
        public void Cleanup()
        {
            //TODO Implement later
        }
    }
}