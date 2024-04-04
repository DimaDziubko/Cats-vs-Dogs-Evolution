using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.AssetProvider;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI.TimelineInfoWindow.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Evolution.Scripts
{
    public class EvolutionService : IEvolutionService
    {
        public event Action<EvolutionViewModel> EvolutionViewModelUpdated;
        public event Action<TravelViewModel> TravelViewModelUpdated;
        public event Action<TimelineInfoData> TimelineInfoDataUpdated;
        public event Action LastAgeOpened;

        private readonly IPersistentDataService _persistentData;

        private readonly IGameConfigController _gameConfigController;

        private readonly IMyLogger _logger;

        private readonly IAssetProvider _assetProvider;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        private EvolutionViewModel _evolutionViewModel;

        private TimelineInfoData _timelineInfoData;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EvolutionService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IMyLogger logger,
            IAssetProvider assetProvider)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _logger = logger;
            _assetProvider = assetProvider;
        }

        public async UniTask Init()
        {
            await PrepareTimelineInfoData();
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
        }

        private void OnNextTimelineOpened()
        {
            //TODO Implement later
        }

        private async UniTask PrepareTimelineInfoData()
        {
            var ageConfigs = _gameConfigController.GetAgeConfigs();

            var ct = _cts.Token;
            try
            {
                _timelineInfoData = new TimelineInfoData()
                {
                    CurrentAge = TimelineState.AgeId,
                    Models = new List<TimelineInfoItemModel>(6)
                };

                foreach (var config in ageConfigs)
                {
                    var model = new TimelineInfoItemModel
                    {
                        Name = config.Name, 
                        Description = config.Description, 
                        DateRange = config.DateRange
                    };
                    
                    ct.ThrowIfCancellationRequested();
                    model.AgeIcon = await _assetProvider.Load<Sprite>(config.AgeIconKey);
                    
                    _timelineInfoData.Models.Add(model);
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
        }

        public void MoveToNextAge()
        {
            if (IsNextAge())
            {
                _persistentData.OpenNextAge();
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

        public void OnTimelineInfoWindowOpened()
        {
            TimelineInfoDataUpdated?.Invoke(_timelineInfoData);
        }
        
        public bool IsTimeToTravel() => !IsNextAge();

        public void OnEvolutionTabOpened() => UpdateEvolutionData();

        public void OnTravelTabOpened() => UpdateTravelData();

        private void UpdateTravelData()
        {
            int timelineNumberOffset = 2;
            
            var travelViewModel = new TravelViewModel()
            {
                NextTimelineNumber = GetTimelineId() + timelineNumberOffset,
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

            _evolutionViewModel = new EvolutionViewModel()
            {
                CurrentTimelineId = GetTimelineId(),
                CurrentAgeIcon = _timelineInfoData.Models[currentAgeNumber].AgeIcon,
                EvolutionBtnData = evolutionButtonData,
                CurrentAgeName = _timelineInfoData.Models[currentAgeNumber].Name,
            };

            if (IsNextAge())
            {
                _evolutionViewModel.NextAgeIcon = _timelineInfoData.Models[nextAgeNumber].AgeIcon;
                _evolutionViewModel.NextAgeName = _timelineInfoData.Models[nextAgeNumber].Name;
            }
            
            EvolutionViewModelUpdated?.Invoke(_evolutionViewModel);
        }

        private bool IsNextAge() => 
            TimelineState.AgeId < _gameConfigController.LastAge();

        private int GetTimelineId() => TimelineState.TimelineId;

        private bool IsNextAgeAffordable()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId) <= Currency.Coins;
        }

        private float GetEvolutionPrice()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId);
        }
    }
    
}