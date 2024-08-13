using System;
using System.Collections.Generic;
using _Game.Core.UserState._State;
using Assets._Game.Core.Communication;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo120 : StateMigrationBase
    {
        public override string TargetVersion => "1.2.0";

        public override void Migrate(ref UserAccountState state)
        {
            state.AdsWeeklyWatchState ??= new AdsWeeklyWatchState()
            {
                LastWeekAdsWatched = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 },
                LastDay = DateTime.Today
            };
        }
    }
}