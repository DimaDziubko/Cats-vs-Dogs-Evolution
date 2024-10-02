using _Game.Core.UserState._State;

namespace _Game.Core.Communication.Migrations
{
    public class MigrationTo140 : StateMigrationBase
    {
        public override string TargetVersion => "1.4.0";

        public override void Migrate(ref UserAccountState state)
        {
            state.FreeGemsPackContainer ??= new FreeGemsPackContainer();
            state.AdsGemsPackContainer ??= new AdsGemsPackContainer();
        }
    }
}