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
        private const string PP_RATE_GAME_CLICKED = "is_rate_game_clicked_save";

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

            //TODO OnPostInitialization run Faster than subscribe on event
            //gameInitializer.OnPostInitialization += Init;

            Init();
            Debug.Log("RateGame INIT");
        }

        private void Init()
        {
            TimelineStateReadonly.NextBattleOpened += OnNextBattleOpen;
        }

        public void Dispose()
        {
            //_gameInitializer.OnPostInitialization -= Init;
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
            //Check if work that subscription
            screen.Value.OnSetPP += SetPPValue;

            var isDecision = await screen.Value.AwaitForDecision();
            if (isDecision)
            {
                screen.Value.OnSetPP -= SetPPValue;
                screen.Dispose();
            }
        }

        private void SetPPValue()
        {
            PlayerPrefs.SetString(PP_RATE_GAME_CLICKED, "true");
        }
    }
}