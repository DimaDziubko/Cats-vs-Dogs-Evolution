﻿using System;
using System.Collections.Generic;
using _Game.Common;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Currencies;
using _Game.UI._Shop.Scripts._AdsGemsPack;
using _Game.Utils.Timers;
using Assets._Game.Gameplay._Timer.Scripts;
using CAS;

#if cas_advertisment_enabled
using CAS;
#endif

namespace _Game.Core.Services._AdsGemsPackService
{
    public class AdsGemsPackService : IAdsGemsPackService, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IAdsService _adsService;
        private readonly IGameInitializer _gameInitializer;
        private readonly TimeBasedRecoveryCalculator _recoveryCalculator;

        private IAdsGemsPackContainer AdsGemsPackContainer => _userContainer.State.AdsGemsPackContainer;

        private readonly Dictionary<int, AdsGemsPack> _adsGemsPacks = new Dictionary<int, AdsGemsPack>();
        private readonly Dictionary<int, SynchronizedCountdownTimer> _countdownTimers = new Dictionary<int, SynchronizedCountdownTimer>();

        public AdsGemsPackService(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IAdsService adsService,
            IGameInitializer gameInitializer,
            TimeBasedRecoveryCalculator recoveryCalculator)
        {
            _userContainer = userContainer;
            _shopConfigRepository = configRepositoryFacade.ShopConfigRepository;
            _logger = logger;
            _adsService = adsService;
            _gameInitializer = gameInitializer;
            _recoveryCalculator = recoveryCalculator;

            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            CheckStorage();
            InitAdsGemsPacks();
            _adsService.VideoLoaded += OnRewardVideoLoaded;

            foreach (var pack in AdsGemsPackContainer.AdsGemsPacks.Values)
            {
                pack.AdsGemsPackCountChanged += OnAdsGemsPackCountChanged;
            }

        }

        void IDisposable.Dispose()
        {
            _adsService.VideoLoaded -= OnRewardVideoLoaded;
            _gameInitializer.OnPostInitialization -= Init;

            foreach (var pack in AdsGemsPackContainer.AdsGemsPacks.Values)
            {
                pack.AdsGemsPackCountChanged -= OnAdsGemsPackCountChanged;
            }

            _adsGemsPacks.Clear();

            foreach (var timer in _countdownTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            
            _countdownTimers.Clear();
        }

        public Dictionary<int, AdsGemsPack> GetAdsGemsPacks() => _adsGemsPacks;

        private void OnAdsGemsPackCountChanged(int id, int newCount)
        {
            _adsGemsPacks[id].SetAmount(newCount);
            CheckRecovering(AdsGemsPackContainer.AdsGemsPacks[id], _adsGemsPacks[id]);
        }

        private void CheckStorage()
        {
            if (AdsGemsPackContainer == null)
                _userContainer.State.AdsGemsPackContainer = new AdsGemsPackContainer();

            HashSet<int> validPackIds = new HashSet<int>();

            foreach (AdsGemsPackConfig config in _shopConfigRepository.GetAdsGemsPackConfigs())
            {
                validPackIds.Add(config.Id);

                if (!AdsGemsPackContainer.TryGetPack(config.Id, out var pack))
                {
                    var newPack = new AdsGemsPackState()
                    {
                        Id = config.Id,
                        AdsGemPackCount = config.DailyGemsPackCount,
                        LastAdsGemPackDay = DateTime.UtcNow
                    };

                    AdsGemsPackContainer.AddPack(config.Id, newPack);
                }
            }

            var packsToRemove = new List<int>();

            foreach (var existingPackId in AdsGemsPackContainer.AdsGemsPacks.Keys)
            {
                if (!validPackIds.Contains(existingPackId))
                {
                    packsToRemove.Add(existingPackId);
                }
            }

            foreach (var packId in packsToRemove)
            {
                AdsGemsPackContainer.RemovePack(packId);
            }
        }

        private void InitAdsGemsPacks()
        {
            _adsGemsPacks.Clear();

            foreach (var config in _shopConfigRepository.GetAdsGemsPackConfigs())
            {
                AdsGemsPackState packState = AdsGemsPackContainer.AdsGemsPacks[config.Id];

                AdsGemsPack pack = new AdsGemsPack
                {
                    Id = config.Id,
                    Config = config,
                };

                CheckRecovering(packState, pack);

                pack.SetLoaded(_adsService.IsAdReady(AdType.Rewarded));
                pack.SetAmount(packState.AdsGemPackCount);

                _adsGemsPacks.Add(pack.Id, pack);
            }
        }

        private void CheckRecovering(AdsGemsPackState packState, AdsGemsPack pack)
        {
            if (packState.AdsGemPackCount > pack.Config.DailyGemsPackCount)
            {
                int delta = pack.Config.DailyGemsPackCount - packState.AdsGemPackCount;
                _userContainer.AdsGemsPackStateHandler.RecoverAdsGemsPack(pack.Id, delta, DateTime.UtcNow);
                return;
            }

            if (packState.AdsGemPackCount == pack.Config.DailyGemsPackCount) return;

            var isRecovered = _recoveryCalculator.CalculateRecoveredUnits(
                packState.AdsGemPackCount,
                pack.Config.DailyGemsPackCount,
                pack.Config.RecoverTimeMinutes,
                packState.LastAdsGemPackDay,
                out int recoveredUnits);

            if (isRecovered)
            {
                _userContainer.AdsGemsPackStateHandler.RecoverAdsGemsPack(pack.Id, recoveredUnits, DateTime.UtcNow);
                return;
            }

            if (recoveredUnits > 0)
            {
                DateTime newLastUseTime = _recoveryCalculator.CalculateNewLastUseTime(
                    packState.LastAdsGemPackDay,
                    recoveredUnits,
                    pack.Config.RecoverTimeMinutes);

                _userContainer.AdsGemsPackStateHandler.RecoverAdsGemsPack(pack.Id, recoveredUnits, newLastUseTime);
            }


            float timeUntilNextRecover = _recoveryCalculator.CalculateTimeUntilNextRecoverySeconds(pack.Config.RecoverTimeMinutes, packState.LastAdsGemPackDay);

            TimeSpan timeSpanUntilNextRecover = TimeSpan.FromSeconds(timeUntilNextRecover);

            _logger.Log($"[CheckRecovering] Time Until Next Recovery: {timeSpanUntilNextRecover:hh\\:mm\\:ss}", DebugStatus.Success);

            StartRecoveryTimer(pack, timeUntilNextRecover);


            _logger.Log($"[CheckRecovering] Pack ID: {pack.Id}, Is Recovered: {isRecovered}, Recovered Units: {recoveredUnits}", DebugStatus.Success);
            _logger.Log($"[CheckRecovering] Recover Time (minutes): {pack.Config.RecoverTimeMinutes}, Max Packs: {pack.Config.DailyGemsPackCount}, Current Packs: {packState.AdsGemPackCount}", DebugStatus.Success);

        }

        private void StartRecoveryTimer(AdsGemsPack pack, float remainingTime)
        {

            if (!_countdownTimers.TryGetValue(pack.Id, out var timer))
            {
                _countdownTimers[pack.Id]  = new SynchronizedCountdownTimer(remainingTime);
                _countdownTimers[pack.Id].TimerStop += () => RecoverPack(pack);
            }
            
            _countdownTimers[pack.Id].Reset(remainingTime);
            _countdownTimers[pack.Id].OnTick += pack.Tick;
            _countdownTimers[pack.Id].Start();
            pack.Tick(_countdownTimers[pack.Id].CurrentTime);
        }

        private void RecoverPack(AdsGemsPack pack)
        {
            _countdownTimers[pack.Id] .OnTick -= pack.Tick;
            _countdownTimers[pack.Id] .Stop();

            var packState = AdsGemsPackContainer.AdsGemsPacks[pack.Id];
            if (packState.AdsGemPackCount < pack.Config.DailyGemsPackCount)
            {
                CheckRecovering(packState, pack);
            }
        }


        private void OnRewardVideoLoaded(AdType type)
        {
            if (type == AdType.Rewarded)
            {
                foreach (var pack in _adsGemsPacks)
                {
                    pack.Value.SetLoaded(_adsService.IsAdReady(AdType.Rewarded));
                }
            }
        }

        void IAdsGemsPackService.OnAdsGemsPackBtnClicked(AdsGemsPack pack)
        {
            if (_adsService.IsAdReady(AdType.Rewarded))
            {
                _adsService.ShowRewardedVideo(() => OnComplete(pack), Placement.FreeGemsPack);
            }
            else
            {
                _logger.Log("Rewarded video ad is not ready", DebugStatus.Warning);
            }
        }

        private void OnComplete(AdsGemsPack pack)
        {
            _userContainer.AdsGemsPackStateHandler.SpendAdsGemsPack(pack.Id, DateTime.UtcNow);
            _userContainer.CurrenciesHandler.AddGems(pack.Config.Quantity, CurrenciesSource.AdsGemsPack);
        }
    }
}