using System.Collections.Generic;
using _Game.Core.Services.IAP;
using _Game.Core.UserState;
using Assets._Game.Core.Communication;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo110 : StateMigrationBase
    {
        public override string TargetVersion => "1.1.0";

        public override void Migrate(ref UserAccountState state)
        {
            state.PurchaseDataState ??= new PurchaseDataState()
            {
                BoudhtIAPs = new List<BoughtIAP>()
            };
        }
    }
}