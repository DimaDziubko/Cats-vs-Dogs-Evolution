﻿using System;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.UI._StartBattleWindow.Scripts;

namespace _Game.Core.Navigation.Battle
{
    public class BattleNavigator : IBattleNavigator, IDisposable
    {
        public event Action BattleChanged;
        public event Action<BattleNavigationModel> NavigationUpdated;
        
        private readonly ITimelineConfigRepository _timelineConfig;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private int _currentBattle;

        public int CurrentBattle
        {
            get => _currentBattle;
            private set
            {
                _currentBattle = value;
                BattleChanged?.Invoke();
                NavigationUpdated?.Invoke(NavigationModel);
            }
        }
        
        private BattleNavigationModel NavigationModel =>
            new BattleNavigationModel()
            {
                CanMoveToNextBattle = CanMoveToNextBattle(),
                CanMoveToPreviousBattle = CanMoveToPreviousBattle(),
                IsPrepared = true
            };
        

        public BattleNavigator(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _timelineConfig = configRepositoryFacade.TimelineConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _ageNavigator.AgeChanged += OnAgeChanged;
            CurrentBattle = TimelineState.MaxBattle;
        }

        void IDisposable.Dispose()
        {
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }
        
        private void OnAgeChanged() => CurrentBattle = TimelineState.MaxBattle;

        public void ForceMoveToNextBattle() => CurrentBattle++;

        public void MoveToNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                CurrentBattle++;
            }
        }

        public void MoveToPreviousBattle()
        {
            if (CanMoveToPreviousBattle())
            {
                CurrentBattle--;
            }
        }

        public void OpenNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                MoveToNextBattle();
                return;
            }

            if (IsLastBattle()) _userContainer.TimelineStateHandler.SetAllBattlesWon(true);

            if (!IsLastBattle() && !CanMoveToNextBattle())
            {
                _userContainer.TimelineStateHandler.OpenNewBattle(CurrentBattle + 1);
                MoveToNextBattle();
            }
        }

        public void OnStartBattleWindowOpened() => NavigationUpdated?.Invoke(NavigationModel);

        private bool CanMoveToNextBattle() => CurrentBattle < TimelineState.MaxBattle;

        public bool CanMoveToPreviousBattle() => CurrentBattle > 0;

        private bool IsLastBattle() => CurrentBattle == _timelineConfig.LastBattle();
    }
}