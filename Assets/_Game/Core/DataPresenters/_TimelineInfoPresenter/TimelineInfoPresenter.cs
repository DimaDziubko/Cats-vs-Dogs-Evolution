using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Configs.Repositories;
using Assets._Game.Core.Data;
using Assets._Game.Core.UserState;
using Assets._Game.UI.TimelineInfoWindow.Scripts;

namespace Assets._Game.Core.DataPresenters._TimelineInfoPresenter
{
    public class TimelineInfoPresenter : ITimelineInfoPresenter, IDisposable
    {
        public event Action<TimelineInfoModel> TimelineInfoDataUpdated;

        private readonly IUserContainer _persistentData;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        
        private TimelineInfoModel _timelineInfoModel;

        public TimelineInfoPresenter(
            IUserContainer persistentData,
            ITimelineConfigRepository timelineConfigRepository,
            IMyLogger logger,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer)
        {
            _persistentData = persistentData;
            _timelineConfigRepository = timelineConfigRepository;
            _logger = logger;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            PrepareTimelineInfoData();
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnNextAgeOpened() => UpdateTimelineInfoData();
        private void OnNextTimelineOpened() => UpdateTimelineInfoData();

        private void  PrepareTimelineInfoData()
        {
            _timelineInfoModel = new TimelineInfoModel()
            {
                CurrentAge = TimelineState.AgeId,
                Models = new List<TimelineInfoItemModel>(6)
            };

            int ageIndex = 0;
            int nextAgeIndex = TimelineState.AgeId + 1;

            for (int i = 0; i < _timelineConfigRepository.GetAgeConfigs().Count(); i++)
            {
                var model = new TimelineInfoItemModel
                {
                    StaticData = _generalDataPool.TimelineStaticData.ForInfoItem(ageIndex),
                    IsUnlocked = nextAgeIndex >= ageIndex
                };
                    
                _timelineInfoModel.Models.Add(model);
                ageIndex++;
            }
        }

        private void UpdateTimelineInfoData()
        {
            _timelineInfoModel.CurrentAge = TimelineState.AgeId;

            int ageIndex = 0;
            int nextAgeIndex = TimelineState.AgeId + 1;
            
            foreach (var model in _timelineInfoModel.Models)
            {
                model.IsUnlocked = nextAgeIndex >= ageIndex;
                ageIndex++;
            }
        }
        

        void ITimelineInfoPresenter.OnTimelineInfoWindowOpened() => 
            TimelineInfoDataUpdated?.Invoke(_timelineInfoModel);

    }
}