using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI.Pin.Scripts;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Evolution.Scripts
{
    public class EvolutionService : IEvolutionService, IDisposable
    {
        public event Action<EvolutionTabData> EvolutionViewModelUpdated;
        public event Action<TravelTabData> TravelViewModelUpdated;
        public event Action<TimelineInfoData> TimelineInfoDataUpdated;
        public event Action LastAgeOpened;

        private readonly IPersistentDataService _persistentData;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IAgeConfigRepository _ageConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IAssetProvider _assetProvider;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        private EvolutionTabData _evolutionTabData;

        private TimelineInfoData _timelineInfoData;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EvolutionService(
            IPersistentDataService persistentData,
            ITimelineConfigRepository timelineConfigRepository,
            IAgeConfigRepository ageConfigRepository,
            IMyLogger logger,
            IAssetProvider assetProvider,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _persistentData = persistentData;
            _timelineConfigRepository = timelineConfigRepository;
            _ageConfigRepository = ageConfigRepository;
            _logger = logger;
            _assetProvider = assetProvider;
            _upgradesChecker = upgradesChecker;
        }

        public async UniTask Init()
        {
            TravelViewModelUpdated += _upgradesChecker.OnTravelModelUpdated;
            EvolutionViewModelUpdated += _upgradesChecker.OnEvolutionModelUpdated;
            await PrepareTimelineInfoData();
            UpdateTravelData();
            UpdateEvolutionData();
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            Currency.CoinsChanged += OnCoinsChanged;
        }

        public void Dispose()
        {
            _cts?.Dispose();
            TravelViewModelUpdated -= _upgradesChecker.OnTravelModelUpdated;
            EvolutionViewModelUpdated -= _upgradesChecker.OnEvolutionModelUpdated;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            Currency.CoinsChanged -= OnCoinsChanged;
        }

        private void OnCoinsChanged(bool _)
        {
            UpdateEvolutionData();
        }

        private void OnNextTimelineOpened()
        {
            //TODO Implement later
        }

        private async UniTask PrepareTimelineInfoData()
        {
            var ageConfigs = _timelineConfigRepository.GetAgeConfigs();

            var ct = _cts.Token;
            try
            {
                _timelineInfoData = new TimelineInfoData()
                {
                    CurrentAge = TimelineState.AgeId,
                    Models = new List<TimelineInfoItemModel>(6)
                };

                int ageIndex = 0;
                int nextAgeIndex = TimelineState.AgeId + 1;
                
                foreach (var config in ageConfigs)
                {
                    var model = new TimelineInfoItemModel
                    {
                        Name = config.Name, 
                        Description = config.Description, 
                        DateRange = config.DateRange,
                        IsUnlocked = nextAgeIndex >= ageIndex
                    };
                    
                    ct.ThrowIfCancellationRequested();
                    model.AgeIcon = await _assetProvider.Load<Sprite>(config.AgeIconKey);
                    
                    _timelineInfoData.Models.Add(model);

                    ageIndex++;
                }
                
                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareTimelineInfoData was canceled.");
            }
        }

        private void OnNextAgeOpened()
        {
            UpdateEvolutionData();
            UpdateTravelData();
            UpdateTimelineInfoData();
            
            if(!IsNextAge()) LastAgeOpened?.Invoke();
        }

        private void UpdateTimelineInfoData()
        {
            _timelineInfoData.CurrentAge = TimelineState.AgeId;

            int ageIndex = 0;
            int nextAgeIndex = TimelineState.AgeId + 1;
            
            foreach (var model in _timelineInfoData.Models)
            {
                model.IsUnlocked = nextAgeIndex >= ageIndex;
                ageIndex++;
            }
        }

        public void MoveToNextAge()
        {
            if (IsNextAge())
            {
                _persistentData.OnOpenNextAge();
            }
            else
            {
                _persistentData.OpenNextTimeline();
            }
        }

        public void MoveToNextTimeline()
        {
            //TODO Implement later
        }

        public void OnTimelineInfoWindowOpened() => 
            TimelineInfoDataUpdated?.Invoke(_timelineInfoData);

        bool IEvolutionService.IsTimeToTravel() => !IsNextAge();

        void IEvolutionService.OnEvolutionTabOpened() => UpdateEvolutionData();

        void IEvolutionService.OnTravelTabOpened() => UpdateTravelData();

        private void UpdateTravelData()
        {
            int timelineNumberOffset = 2;
            
            var travelViewModel = new TravelTabData()
            {
                NextTimelineNumber = TimelineState.TimelineId + timelineNumberOffset,
                CanTravel = TimelineState.AllBattlesWon,
            };
            
            TravelViewModelUpdated?.Invoke(travelViewModel);
        }

        private void UpdateEvolutionData()
        {
            int currentAgeNumber = TimelineState.AgeId;
            int nextAgeNumber = TimelineState.AgeId + 1;
            
            EvolutionBtnData evolutionButtonData = new EvolutionBtnData()
            {
                CanAfford = IsNextAgeAffordable(),
                Price = GetEvolutionPrice()
            };

            _evolutionTabData = new EvolutionTabData()
            {
                CurrentTimelineId = TimelineState.TimelineId,
                CurrentAgeIcon = _timelineInfoData.Models[currentAgeNumber].AgeIcon,
                EvolutionBtnData = evolutionButtonData,
                CurrentAgeName = _timelineInfoData.Models[currentAgeNumber].Name,
            };

            if (IsNextAge())
            {
                _evolutionTabData.NextAgeIcon = _timelineInfoData.Models[nextAgeNumber].AgeIcon;
                _evolutionTabData.NextAgeName = _timelineInfoData.Models[nextAgeNumber].Name;
            }
            
            EvolutionViewModelUpdated?.Invoke(_evolutionTabData);
        }

        private bool IsNextAge() => 
            TimelineState.AgeId < _timelineConfigRepository.LastAge();

        private bool IsNextAgeAffordable() =>
            _ageConfigRepository
                .GetAgePrice(TimelineState.AgeId) <= Currency.Coins;

        private float GetEvolutionPrice() =>
            _ageConfigRepository
                .GetAgePrice(TimelineState.AgeId);
    }
    
}