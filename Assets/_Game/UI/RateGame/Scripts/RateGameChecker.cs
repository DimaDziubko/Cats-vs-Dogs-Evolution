using System;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core.UserState;
using UnityEngine;

namespace _Game.UI.RateGame.Scripts
{
    public interface IRateGameChecker
    {
    }

    public class RateGameChecker : IRateGameChecker, IDisposable
    {
        private const string PP_RATE_GAME_CLICKED = "isrategameclicked_save";
        
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IRateGameProvider _rateGameProvider;


        private ITimelineStateReadonly TimelineStateReadonly => _userContainer.State.TimelineState;
        
        public RateGameChecker(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IRateGameProvider rateGameProvider)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _rateGameProvider = rateGameProvider;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init() => TimelineStateReadonly.NextBattleOpened += OnNextBattleOpen;


        public void Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
            TimelineStateReadonly.NextBattleOpened -= OnNextBattleOpen;
        }

        private void OnNextBattleOpen()
        {
            if (IsTimeToShow() && !IsReviewed())
            {
                ShowScreen();
            }
        }

        private bool IsReviewed() => PlayerPrefs.HasKey(PP_RATE_GAME_CLICKED);

        private bool IsTimeToShow()
        {
            return TimelineStateReadonly.TimelineId == 0 && TimelineStateReadonly.MaxBattle == 1 ||
                   TimelineStateReadonly.TimelineId == 0 && TimelineStateReadonly.MaxBattle == 2 ||
                   TimelineStateReadonly.TimelineId == 1 && TimelineStateReadonly.MaxBattle == 1;
        }

        private async void ShowScreen()
        {
            var screen = await _rateGameProvider.Load();
            var isDecision = await screen.Value.AwaitForDecision();
            if(isDecision) screen.Dispose();
        }
    }
}