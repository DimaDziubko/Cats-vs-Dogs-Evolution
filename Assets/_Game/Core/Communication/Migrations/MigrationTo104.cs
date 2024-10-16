﻿using System;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using Assets._Game.Core.Communication;
using Assets._Game.Core.UserState;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo104 : StateMigrationBase
    {
        public override string TargetVersion => "1.0.4";

        public override void Migrate(ref UserAccountState state)
        {
            state.AdsStatistics ??= new AdsStatistics
            {
                AdsReviewed = 0
            };

            state.RetentionState ??= new RetentionState
            {
                FirstOpenTime = DateTime.UtcNow,
                FirstDayRetentionEventSent = false,
                SecondDayRetentionEventSent = false
            };
        }
    }
}