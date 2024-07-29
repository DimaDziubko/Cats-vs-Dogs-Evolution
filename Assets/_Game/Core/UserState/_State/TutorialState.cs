using System;

namespace Assets._Game.Core.UserState
{
    public class TutorialState : ITutorialStateReadonly 
    {
        public int StepsCompleted;

        public event Action<int> StepsCompletedChanged;

        int ITutorialStateReadonly.StepsCompleted => StepsCompleted;

        public void ChangeCompletedStep(int step)
        {
            StepsCompleted = step;    
            StepsCompletedChanged?.Invoke(step);
        }
    }

    public interface ITutorialStateReadonly
    {
        int StepsCompleted { get; }
        event Action<int> StepsCompletedChanged;
    }
}