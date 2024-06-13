﻿using System;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI._StartBattleWindow.Scripts;

namespace _Game.Core.Navigation.Battle
{
    public class BattleNavigator : IBattleNavigator, IDisposable
    {
        public event Action BattleChanged;

        public event Action<BattleNavigationModel> NavigationUpdated;
        private readonly ITimelineConfigRepository _timelineConfig;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
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
            ITimelineConfigRepository timelineConfig,
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _timelineConfig = timelineConfig;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            CurrentBattle = TimelineState.MaxBattle;
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnNextAgeOpened() => CurrentBattle = TimelineState.MaxBattle;

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

            if (IsLastBattle()) _userContainer.SetAllBattlesWon(true);

            if (!IsLastBattle() && !CanMoveToNextBattle())
            {
                _userContainer.OpenNextBattle(CurrentBattle + 1);
                MoveToNextBattle();
            }
        }

        public void OnStartBattleWindowOpened() => NavigationUpdated?.Invoke(NavigationModel);

        private bool IsLastBattle() => CurrentBattle == _timelineConfig.LastBattle();

        private bool CanMoveToNextBattle() => CurrentBattle < TimelineState.MaxBattle;

        private bool CanMoveToPreviousBattle() => CurrentBattle > 0;
    }
}