using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Common.Animation.Scripts
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        AnimatorState State { get; }
    }
}