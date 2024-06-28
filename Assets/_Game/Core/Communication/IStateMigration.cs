using _Game.Core.UserState;

namespace _Game.Core.Communication
{
    public interface IStateMigration
    {
        string TargetVersion { get; }
        void Migrate(ref UserAccountState state);
    }
}