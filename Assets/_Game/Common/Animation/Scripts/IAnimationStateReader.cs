using _Game.Bundles.Units.Common.Scripts;

namespace _Game.Common.Animation.Scripts
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        AnimatorState State { get; }
    }
}