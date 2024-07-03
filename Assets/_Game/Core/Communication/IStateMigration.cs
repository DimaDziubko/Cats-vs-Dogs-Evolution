using Assets._Game.Core.UserState;

namespace Assets._Game.Core.Communication
{
    public interface IStateMigration
    {
        string TargetVersion { get; }
        void Migrate(ref UserAccountState state);
    }
}