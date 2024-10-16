﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.DataPresenters._TimelineInfoPresenter;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.UI.TimelineInfoScreen.Scripts;
using _Game.Utils.Extensions;
using Assets._Game.Core.UserState;
using Assets._Game.UI.TimelineInfoWindow.Scripts;

namespace _Game.Core._DataPresenters._TimelineInfoPresenter
{
    public class TimelineInfoPresenter : ITimelineInfoPresenter, IDisposable
    {
        public event Action<TimelineInfoModel> TimelineInfoDataUpdated;

        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IDifficultyConfigRepository _difficultyConfig;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private TimelineInfoModel _timelineInfoModel;

        public TimelineInfoPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IGeneralDataPool generalDataPool,
            IGameInitializer gameInitializer,
            ITimelineNavigator timelineNavigator)
        {
            _userContainer = userContainer;
            _timelineConfigRepository = configRepositoryFacade.TimelineConfigRepository;
            _difficultyConfig = configRepositoryFacade.DifficultyConfigRepository;
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
                TimelineInfo = $"Timeline {TimelineState.TimelineId + 1}",
                DifficultyInfo = $"Difficulty x{_difficultyConfig.GetDifficultyValue(TimelineState.TimelineId + 1).ToFormattedString()}",
                ShowDifficulty = _difficultyConfig.GetDifficultyValue(TimelineState.TimelineId + 1) > 1,
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
            _timelineInfoModel.TimelineInfo = $"Timeline {TimelineState.TimelineId + 1}";
            _timelineInfoModel.DifficultyInfo =
                $"Difficulty x{_difficultyConfig.GetDifficultyValue(TimelineState.TimelineId + 1).ToFormattedString()}";
            _timelineInfoModel.ShowDifficulty = _difficultyConfig.GetDifficultyValue(TimelineState.TimelineId + 1) > 1;

            int ageIndex = 0;
            int nextAgeIndex = TimelineState.AgeId + 1;

            foreach (var model in _timelineInfoModel.Models)
            {
                model.IsUnlocked = nextAgeIndex >= ageIndex;
                //UnityEngine.Debug.Log("nextAgeIndex " + nextAgeIndex + "__ ageIndex " + ageIndex);
                ageIndex++;
            }
        }

        void ITimelineInfoPresenter.OnTimelineInfoScreenOpened() =>
                TimelineInfoDataUpdated?.Invoke(_timelineInfoModel);

        void ITimelineInfoPresenter.OnPrepareTimelineInfoData()
        {
            PrepareTimelineInfoData();
        }
    }
}