﻿using System;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Ads;
using UnityEngine;

namespace _Game.Core.Services._FreeGemsPackService
{
    public class FreeGemsPackService : IFreeGemsPackService, IDisposable
    {
        public event Action FreeGemsPackUpdated;
        
        private readonly IUserContainer _userContainer;
        private readonly IEconomyConfigRepository _economyConfigRepository;
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IAdsService _adsService;
        private readonly IGameInitializer _gameInitializer;
        private IFreeGemsPackStateReadonly FreeGemsPackState => _userContainer.State.FreeGemsPackState;

        public ProductDescription FreeGemsPack => MakeProductDefinition();

        public FreeGemsPackService(
            IUserContainer userContainer,
            IEconomyConfigRepository economyConfigRepository,
            IShopConfigRepository shopConfigRepository,
            IMyLogger logger,
            IAdsService adsService,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _economyConfigRepository = economyConfigRepository;
            _shopConfigRepository = shopConfigRepository;
            _logger = logger;
            _adsService = adsService;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            UpgradeFreeGemsPack();
            _adsService.RewardedVideoLoaded += OnRewardVideoLoaded;

        }

        void IDisposable.Dispose()
        {
            _adsService.RewardedVideoLoaded -= OnRewardVideoLoaded;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnRewardVideoLoaded()
        {
            NotifyUpdateFreeGemsPack();
        }

        private void NotifyUpdateFreeGemsPack()
        {
            FreeGemsPackUpdated?.Invoke();
        }


        public void OnFreeGemsPackBtnClicked() => 
            _adsService.ShowRewardedVideo(OnRewardedVideoComplete, RewardType.FreeGemsPack);
        
        private ProductDescription MakeProductDefinition()
        {
            UpgradeFreeGemsPack();
            
            var freeGemsPackDayConfig = _economyConfigRepository.GetFreeGemsPackDayConfig();
            var productConfig = _shopConfigRepository.GetPlacementConfig();
            var productDescription = new ProductDescription()
            {
                Id = productConfig.IAP_ID,
                AvailablePurchasesLeft = FreeGemsPackState.FreeGemPackCount,
                MaxPurchasesCount = freeGemsPackDayConfig.DailyGemsPackCount,
                Config = productConfig,
                IsReady = _adsService.IsRewardedVideoReady,
            };

            return productDescription;
        }

        private void UpgradeFreeGemsPack()
        {
            var freeGemsPackDayConfig = _economyConfigRepository.GetFreeGemsPackDayConfig();
            
            DateTime now = DateTime.UtcNow;

            TimeSpan timeSinceLastBoost = now - FreeGemsPackState.LastFreeGemPackDay;
            int recoverablePacks = (int)(timeSinceLastBoost.TotalMinutes / freeGemsPackDayConfig.RecoverTimeMinutes);

            if (recoverablePacks > 0)
            {
                int lackingPacks = freeGemsPackDayConfig.DailyGemsPackCount - FreeGemsPackState.FreeGemPackCount;
                int packsToAdd = Mathf.Min(recoverablePacks, lackingPacks);
                
                DateTime newLastDailyFreePackSpent = FreeGemsPackState.LastFreeGemPackDay
                    .AddMinutes(packsToAdd * freeGemsPackDayConfig.RecoverTimeMinutes);
                _userContainer.FreeGemsPackStateHandler.RecoverFreeGemsPack(
                    packsToAdd,
                    newLastDailyFreePackSpent);
            }
        }
        
        private void OnRewardedVideoComplete()
        {
            var freeGemsPackConfig = _economyConfigRepository.GetFreeGemsPackDayConfig();
            var productConfig = _shopConfigRepository.GetPlacementConfig();

            _userContainer.FreeGemsPackStateHandler.SpendGemsPack(FreeGemsPackState.FreeGemPackCount == freeGemsPackConfig.DailyGemsPackCount
                ? DateTime.UtcNow
                : FreeGemsPackState.LastFreeGemPackDay);

            _userContainer.CurrenciesHandler.AddGems(productConfig.Quantity);
        }
    }
}