﻿using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Currencies;
using _Game.Utils;
using Assets._Game.Core.UserState;
using DevToDev.Analytics;
using Product = UnityEngine.Purchasing.Product;

namespace _Game.Core.Services.Analytics
{
    public class DTDAnalyticsService : IDTDAnalyticsService, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IAdsService _adsService;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly IIAPService _iapService;
        private readonly IIGPService _igpService;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        
        public DTDAnalyticsService(
            IUserContainer userContainer,
            IAdsService adsService,
            IGameInitializer gameInitializer,
            IMyLogger logger,
            IIAPService iapService,
            IIGPService igpService)
        {
            _userContainer = userContainer;
            _adsService = adsService;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _iapService = iapService;
            _igpService = igpService;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextBattleOpened += OnNextBattleOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.OpenedUnit += OnUnitOpened;
            _adsService.AdImpression += TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged += OnStepCompleted;
            RaceState.Changed += OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            _iapService.Purchased += TrackPurchase;
            _igpService.Purchased += TrackInGamePurchase;
            
            if (!TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.START_TUTORIAL_KEY))
            {
                DTDAnalytics.Tutorial(Constants.TutorialSteps.START_TUTORIAL_KEY);
                _userContainer.TutorialStateHandler.CompleteTutorialStep(Constants.TutorialSteps.START_TUTORIAL_KEY);
            }
        }

        public void Dispose()
        {
            TimelineState.NextBattleOpened -= OnNextBattleOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.OpenedUnit -= OnUnitOpened;
            _adsService.AdImpression -= TrackRewardedVideoAdImpression;
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            RaceState.Changed -= OnRaceChanged;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            _iapService.Purchased -= TrackPurchase;  
            _igpService.Purchased -= TrackInGamePurchase;
            
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void TrackPurchase(Product product)
        {
            if (product == null || product.metadata == null || string.IsNullOrEmpty(product.transactionID))
            {
                _logger.LogWarning("Invalid product data");
                return;
            }
            
            string orderId = product.transactionID;
            double price = (double)product.metadata.localizedPrice;
            string productId = product.definition.id;
            string currencyCode = product.metadata.isoCurrencyCode;
            
            DTDAnalytics.RealCurrencyPayment(orderId, price, productId, currencyCode);
        }

        private void OnCurrenciesChanged(CurrencyType type, double amount, CurrenciesSource source)
        {
            if (amount > 1)
            {
                TrackCurrencyAccrual(type, amount, source);
            }
        }
        
        private void TrackInGamePurchase(IGPDto dto)
        {
            if (dto.Resources != null)
            {
                DTDAnalytics.VirtualCurrencyPayment(dto.PurchaseId, dto.PurchaseType, dto.PurchaseAmount,  dto.Resources);
                return;
            }
            
            DTDAnalytics.VirtualCurrencyPayment(dto.PurchaseId, dto.PurchaseType, dto.PurchaseAmount, dto.PurchasePrice, dto.PurchaseCurrency);
        }

        private void TrackCurrencyAccrual(CurrencyType type, double amount, CurrenciesSource source)
        {
            DTDAccrualType accrualType;
            if (source == CurrenciesSource.Shop || source == CurrenciesSource.MiniShop) accrualType = DTDAccrualType.Bought;
            else
            {
                accrualType = DTDAccrualType.Earned;
            }
            
            switch (type)
            {
                case UI._Currencies.CurrencyType.Coins:
                    break;
                case UI._Currencies.CurrencyType.Gems:
                    DTDAnalytics.CurrencyAccrual(type.ToString(), (int)amount, source.ToString(), accrualType);
                    DTDAnalytics.CurrentBalance(new Dictionary<string, long>
                    {
                        { type.ToString(), (long)Currencies.Gems }
                    });
                    break;
            }
        }
        
        private void OnStepCompleted(int step)
        {
            int lastStep = Constants.TutorialSteps.LAST_STEP;
            if (step == lastStep)
            {
                DTDAnalytics.Tutorial(Constants.TutorialSteps.COMPLETE_TUTORIAL_KEY);
                _userContainer.TutorialStateHandler.CompleteTutorialStep(Constants.TutorialSteps.COMPLETE_TUTORIAL_KEY);
                return;
            }
            DTDAnalytics.Tutorial(step);
        }

        private void TrackRewardedVideoAdImpression(AdImpressionDto dto) => 
            DTDAnalytics.AdImpression(dto.Network, dto.Revenue, dto.Placement.ToString(), dto.UnitId);
        
        private void OnNextAgeOpened()
        {
            var timelineNumber = TimelineState.TimelineId + 1;
            var localAgeNumber = TimelineState.AgeId + 1;
            
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", timelineNumber);
            parameters.Add("age№", localAgeNumber);
            DTDAnalytics.CustomEvent("evolution_completed", parameters);

            var maxAgesCountInTimeline = 6;
            var globalAgeNumber = maxAgesCountInTimeline * (TimelineState.TimelineId) + (localAgeNumber);

            DTDAnalytics.LevelUp(globalAgeNumber);
        }
        
        private void OnNextBattleOpened()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("battle№", TimelineState.MaxBattle);
        
            DTDAnalytics.CustomEvent("battle_completed", parameters);
        }
        
        public void OnBattleStarted(BattleAnalyticsData battleAnalyticsData)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", battleAnalyticsData.TimelineNumber);
            parameters.Add("age№", battleAnalyticsData.AgeNumber);
            parameters.Add("battle№", battleAnalyticsData.BattleNumber);
        
            DTDAnalytics.CustomEvent("battle_started", parameters);
        }
        
        public void SendWave(string wave, BattleAnalyticsData data)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", data.TimelineNumber);
            parameters.Add("age№", data.AgeNumber);
            parameters.Add("battle№", data.BattleNumber);
            parameters.Add("wave№", wave);
            
            DTDAnalytics.CustomEvent("wave_started", parameters);
        }

        
        public void SendEvent(string eventName)
        {
            DTDAnalytics.CustomEvent(eventName);
        }
        
        private void OnUnitOpened(UnitType type)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("unit", (int)type);
        
            DTDAnalytics.CustomEvent("unit_opened", parameters);
        }
        
        private void OnRaceChanged()
        {
            var parameters = new DTDCustomEventParameters();
            
            if (RaceState.Counter == 1)
            {
                DTDAnalytics.CustomEvent($"race_selected {RaceState.CurrentRace.ToString()}");
                _logger.Log("race_selected");
                return;
            }
        
            parameters.Add("timeline№", TimelineState.TimelineId + 1);
            parameters.Add("age№", TimelineState.AgeId + 1);
            parameters.Add("race", RaceState.CurrentRace.ToString());
            DTDAnalytics.CustomEvent("race_changed", parameters);
            _logger.Log("race_changed");
        }
        
        private void OnCompletedBattleChanged()
        {
            if (BattleStatistics.BattlesCompleted == 1 && TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                DTDAnalytics.CustomEvent("first_build_success");
                _logger.Log("first_build_success");
            }
            
            else if (BattleStatistics.BattlesCompleted == 1 && !TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                DTDAnalytics.CustomEvent("first_build_failed");
                _logger.Log("first_build_failed");
            }
        }
    }
}
