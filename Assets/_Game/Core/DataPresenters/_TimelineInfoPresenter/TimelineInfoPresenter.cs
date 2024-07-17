using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.UI.TimelineInfoWindow.Scripts;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data;
using Assets._Game.Core.UserState;
using Assets._Game.UI.TimelineInfoWindow.Scripts;

namespace _Game.Core.DataPresenters._TimelineInfoPresenter
{
    public class TimelineInfoPresenter : ITimelineInfoPresenter, IDisposable
    {
        public event Action<TimelineInfoModel> TimelineInfoDataUpdated;

        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private TimelineInfoModel _timelineInfoModel;
        private TimelineInfoModel _timelineInfoModelForShow;

        public TimelineInfoPresenter(
            IUserContainer userContainer,
            ITimelineConfigRepository timelineConfigRepository,
            IMyLogger logger,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer,
            ITimelineNavigator timelineNavigator)
        {
            _userContainer = userContainer;
            _timelineConfigRepository = timelineConfigRepository;
            _logger = logger;
            _generalDataPool = generalDataPool;
            _gameInitializer = gameInitializer;
            _timelineNavigator = timelineNavigator;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            PrepareTimelineInfoData();
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
        }

        void IDisposable.Dispose()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnNextAgeOpened() => UpdateTimelineInfoData();

        private void OnTimelineChanged()
        {
            PrepareTimelineInfoData();
            UpdateTimelineInfoData();
        }

        private void PrepareTimelineInfoData()
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
                UnityEngine.Debug.Log("nextAgeIndex " + nextAgeIndex + "__ ageIndex " + ageIndex);
                ageIndex++;
            }
        }
        private void PrepareTimelineInfoDataForShow() //ZAGLYSHKA
        {
            //    _timelineInfoModelForShow = new TimelineInfoModel()
            //    {
            //        CurrentAge = TimelineState.AgeId,
            //        Models = new List<TimelineInfoItemModel>(6)
            //    };

            //    int ageIndex = 0;
            //    int nextAgeIndex = TimelineState.AgeId;// + 1;

            //    for (int i = 0; i < _timelineConfigRepository.GetAgeConfigs().Count(); i++)
            //    {
            //        var model = new TimelineInfoItemModel
            //        {
            //            StaticData = _generalDataPool.TimelineStaticData.ForInfoItem(ageIndex),
            //            IsUnlocked = nextAgeIndex >= ageIndex
            //        };

            //        UnityEngine.Debug.Log("nextAgeIndex " + nextAgeIndex + "__ ageIndex " + ageIndex);

            //        _timelineInfoModelForShow.Models.Add(model);
            //        ageIndex++;
            //    }
        }

    void ITimelineInfoPresenter.OnTimelineInfoWindowOpened() =>
            TimelineInfoDataUpdated?.Invoke(_timelineInfoModel);

        void ITimelineInfoPresenter.OnPrepareTimelineInfoDataForShow() =>
                    PrepareTimelineInfoDataForShow();
    }
}