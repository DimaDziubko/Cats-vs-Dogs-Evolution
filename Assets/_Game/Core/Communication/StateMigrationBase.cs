using _Game.Core.UserState;

namespace _Game.Core.Communication
{
    public abstract class StateMigrationBase : IStateMigration
    {
        public abstract string TargetVersion { get; }

        public abstract void Migrate(ref UserAccountState state);
    }
}