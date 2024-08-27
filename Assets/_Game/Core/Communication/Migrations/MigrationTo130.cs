using System.Collections.Generic;
using _Game.Core.UserState._State;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo130 : StateMigrationBase
    {
        public override string TargetVersion => "1.3.0";

        public override void Migrate(ref UserAccountState state)
        {
            state.CardsCollectionState ??= new CardsCollectionState()
            {
                CardSummoningLevel = 1,
                CardsSummoningProgressCount = 0,
                Cards = new List<Card>(),
            };
        }
    }
}