﻿using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._Analytics
{
    public class AnalyticsStateHandler : IAnalyticsStateHandler
    {
        private readonly IUserContainer _userContainer;
        public AnalyticsStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void AddAdsReviewed()
        {
            _userContainer.State.AdsStatistics.AddWatchedAd();
            _userContainer.State.AdsWeeklyWatchState.AddWatchedAd(DateTime.UtcNow);
            _userContainer.RequestSaveGame();

        }

        public void FirstDayRetentionSent()
        {
            _userContainer.State.RetentionState.ChangeFirstDayRetentionEventSent(true);
            _userContainer.RequestSaveGame();
        }

        public void SecondDayRetentionSent()
        {
            _userContainer.State.RetentionState.ChangeSecondDayRetentionEventSent(true);
            _userContainer.RequestSaveGame();

        }

        public void AddCompletedBattle()
        {
            _userContainer.State.BattleStatistics.AddCompletedBattle();
            _userContainer.RequestSaveGame();
        }
    }
}