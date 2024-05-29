using System;
using System.Threading;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Core.UserState;
using _Game.UI._Hud;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services._FoodBoostService.Scripts
{
    public class FoodBoostService : IFoodBoostService, IDisposable
    {
        public event Action<FoodBoostBtnModel> FoodBoostBtnModelChanged;
        public event Action<int> FoodBoost;

        private readonly IPersistentDataService _persistentData;
        private readonly IEconomyConfigRepository _configRepository;
        private readonly IAssetProvider _assetProvider;
        private readonly IEconomyUpgradesService _economyUpgradesService;
        private readonly IMyLogger _logger;
        private readonly IAdsService _adsService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private IFoodBoostStateReadonly FoodBoostState => _persistentData.State.FoodBoost;

        private readonly FoodBoostBtnModel _foodBoostBtnModel = new FoodBoostBtnModel();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public FoodBoostService(
            IPersistentDataService persistentData,
            IEconomyConfigRepository configRepository,
            IAssetProvider assetProvider,
            IEconomyUpgradesService economyUpgradesService,
            IMyLogger logger,
            IAdsService adsService,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _persistentData = persistentData;
            _configRepository = configRepository;
            _assetProvider = assetProvider;
            _economyUpgradesService = economyUpgradesService;
            _logger = logger;
            _adsService = adsService;
            _featureUnlockSystem = featureUnlockSystem;
        }

        public async UniTask Init()
        {
            UpdateFoodBoost();
            await PrepareFoodBoost();

            FoodBoostState.FoodBoostChanged += OnFoodBoostChanged;
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;

        }

        public void Dispose()
        {
            _cts?.Dispose();
            FoodBoostState.FoodBoostChanged -= OnFoodBoostChanged;
            _adsService.RewardedVideoLoaded -= OnRewardVideoLoaded;
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
                _persistentData.RecoverFoodBoost(foodBoostConfig.DailyFoodBoostCount);
                UpdateFoodBoostBtnModel();
            }
        }

        private async UniTask PrepareFoodBoost()
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

        private void OnFoodBoostRewardedVideoComplete()
        {
            _persistentData.SpendFoodBoost();
            FoodBoost?.Invoke(_foodBoostBtnModel.FoodAmount);
        }

        private async UniTask PrepareFoodBoostBtnModel(CancellationToken ct)
        {
            string foodIconKey = _configRepository.GetFoodIconKey();
            ct.ThrowIfCancellationRequested();
            
            _foodBoostBtnModel.FoodIcon = await _assetProvider.Load<Sprite>(foodIconKey);
            UpdateFoodBoostBtnModel();
        }

        private void UpdateFoodBoostBtnModel()
        {
            var foodBoostConfig = _configRepository.GetFoodBoostConfig();
            
            _foodBoostBtnModel.FoodAmount = (int)(_economyUpgradesService.GetFoodProductionSpeed() * foodBoostConfig.FoodBoostCoefficient);
            
            _foodBoostBtnModel.IsAvailable = 
                FoodBoostState.DailyFoodBoostCount > 0 && _featureUnlockSystem.IsFeatureUnlocked(Feature.FoodBoost);
            
            _foodBoostBtnModel.IsInteractable = _adsService.IsRewardedVideoReady;

            FoodBoostBtnModelChanged?.Invoke(_foodBoostBtnModel);
        }
    }
}