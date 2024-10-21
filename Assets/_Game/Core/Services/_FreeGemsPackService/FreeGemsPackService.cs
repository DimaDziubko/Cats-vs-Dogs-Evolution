using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Currencies;
using _Game.UI._Shop.Scripts._FreeGemsPack;
using _Game.Utils.Timers;

namespace _Game.Core.Services._FreeGemsPackService
{
    public class FreeGemsPackService : IFreeGemsPackService, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly TimeBasedRecoveryCalculator _recoveryCalculator;

        private IFreeGemsPackContainer FreeGemsPackContainer => _userContainer.State.FreeGemsPackContainer;

        private readonly Dictionary<int, FreeGemsPack> _freeGemsPacks = new Dictionary<int, FreeGemsPack>();
        private readonly Dictionary<int, SynchronizedCountdownTimer> _countdownTimers = new Dictionary<int, SynchronizedCountdownTimer>();

        public FreeGemsPackService(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IGameInitializer gameInitializer,
            TimeBasedRecoveryCalculator recoveryCalculator)
        {
            _userContainer = userContainer;
            _shopConfigRepository = configRepositoryFacade.ShopConfigRepository;
            _logger = logger;
            _gameInitializer = gameInitializer;
            _recoveryCalculator = recoveryCalculator;
            
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            CheckStorage();
            InitFreeGemsPacks();

            foreach (var pack in FreeGemsPackContainer.FreeGemsPacks.Values)
            {
                pack.FreeGemsPackCountChanged += OnFreeGemsPackCountChanged;
            }

        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
            
            foreach (var pack in FreeGemsPackContainer.FreeGemsPacks.Values)
            {
                pack.FreeGemsPackCountChanged -= OnFreeGemsPackCountChanged;
            }
            
            _freeGemsPacks.Clear();

            foreach (var timer in _countdownTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            
            _countdownTimers.Clear();
        }

        public Dictionary<int, FreeGemsPack> GetFreeGemsPacks() => _freeGemsPacks;

        private void OnFreeGemsPackCountChanged(int id, int newCount)
        {
            _freeGemsPacks[id].SetAmount(newCount);
            CheckRecovering(FreeGemsPackContainer.FreeGemsPacks[id], _freeGemsPacks[id]);
        }

        private void CheckStorage()
        {
            if (FreeGemsPackContainer == null) 
                _userContainer.State.FreeGemsPackContainer = new FreeGemsPackContainer();

            HashSet<int> validPackIds = new HashSet<int>();
            
            foreach (FreeGemsPackConfig config in _shopConfigRepository.GetFreeGemsPackConfigs())
            {
                validPackIds.Add(config.Id);
                
                if (!FreeGemsPackContainer.TryGetPack(config.Id, out var pack))
                {
                    var newPack = new FreeGemsPackState()
                    {
                        Id = config.Id,
                        FreeGemPackCount = config.DailyGemsPackCount,
                        LastFreeGemPackDay = DateTime.UtcNow
                    };
                    
                    FreeGemsPackContainer.AddPack(config.Id, newPack);
                }
            }
            
            var packsToRemove = new List<int>();
            
            foreach (var existingPackId in FreeGemsPackContainer.FreeGemsPacks.Keys)
            {
                if (!validPackIds.Contains(existingPackId))
                {
                    packsToRemove.Add(existingPackId);
                }
            }
            
            foreach (var packId in packsToRemove)
            {
                FreeGemsPackContainer.RemovePack(packId);
            }
        }

        private void InitFreeGemsPacks()
        {
            _freeGemsPacks.Clear();
            
            foreach (var config in _shopConfigRepository.GetFreeGemsPackConfigs())
            {
                FreeGemsPackState packState = FreeGemsPackContainer.FreeGemsPacks[config.Id];
                
                FreeGemsPack pack = new FreeGemsPack
                {
                    Id = config.Id,
                    Config = config,
                };

                CheckRecovering(packState, pack);
                
                pack.SetAmount(packState.FreeGemPackCount);
                
                _freeGemsPacks.Add(pack.Id, pack);
            }
        }

        private void CheckRecovering(FreeGemsPackState packState, FreeGemsPack pack)
        {
            if (packState.FreeGemPackCount > pack.Config.DailyGemsPackCount)
            {
                int delta = pack.Config.DailyGemsPackCount - packState.FreeGemPackCount;
                _userContainer.FreeGemsPackStateHandler.RecoverFreeGemsPack(pack.Id, delta, DateTime.UtcNow); 
                return;
            }
            
            if(packState.FreeGemPackCount == pack.Config.DailyGemsPackCount) return;
            
            var isRecovered = _recoveryCalculator.CalculateRecoveredUnits(
                packState.FreeGemPackCount,
                pack.Config.DailyGemsPackCount,
                pack.Config.RecoverTimeMinutes,
                packState.LastFreeGemPackDay,
                out int recoveredUnits);

            if (isRecovered)
            {
                _userContainer.FreeGemsPackStateHandler.RecoverFreeGemsPack(pack.Id, recoveredUnits, DateTime.UtcNow); 
                return;
            }

            if (recoveredUnits > 0)
            {
                DateTime newLastUseTime = _recoveryCalculator.CalculateNewLastUseTime(
                    packState.LastFreeGemPackDay, 
                    recoveredUnits, 
                    pack.Config.RecoverTimeMinutes);
                
                _userContainer.AdsGemsPackStateHandler.RecoverAdsGemsPack(pack.Id, recoveredUnits, newLastUseTime); 
            }

            
            float timeUntilNextRecover = _recoveryCalculator.CalculateTimeUntilNextRecoverySeconds(pack.Config.RecoverTimeMinutes, packState.LastFreeGemPackDay);

            TimeSpan timeSpanUntilNextRecover = TimeSpan.FromSeconds(timeUntilNextRecover);

            _logger.Log($"[CheckRecovering] Time Until Next Recovery: {timeSpanUntilNextRecover:hh\\:mm\\:ss}", DebugStatus.Success);

            StartRecoveryTimer(pack, timeUntilNextRecover);


            _logger.Log($"[CheckRecovering] Pack ID: {pack.Id}, Is Recovered: {isRecovered}, Recovered Units: {recoveredUnits}", DebugStatus.Success);
            _logger.Log($"[CheckRecovering] Recover Time (minutes): {pack.Config.RecoverTimeMinutes}, Max Packs: {pack.Config.DailyGemsPackCount}, Current Packs: {packState.FreeGemPackCount}", DebugStatus.Success);

        }

        private void StartRecoveryTimer(FreeGemsPack pack, float remainingTime)
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

        private void RecoverPack(FreeGemsPack pack)
        {
            _countdownTimers[pack.Id] .OnTick -= pack.Tick;
            _countdownTimers[pack.Id] .Stop();

            var packState = FreeGemsPackContainer.FreeGemsPacks[pack.Id];
            if (packState.FreeGemPackCount < pack.Config.DailyGemsPackCount)
            {
                CheckRecovering(packState, pack);
            }
        }
        
        void IFreeGemsPackService.OnFreeGemsPackBtnClicked(FreeGemsPack pack)
        {
            _userContainer.FreeGemsPackStateHandler.SpendFreeGemsPack(pack.Id, DateTime.UtcNow);
            _userContainer.CurrenciesHandler.AddGems(pack.Config.Quantity, CurrenciesSource.FreeGemsPack);
        }
    }
}