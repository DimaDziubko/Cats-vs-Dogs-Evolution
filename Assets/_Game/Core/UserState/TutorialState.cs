using System;

namespace _Game.Core.UserState
{
    public class TutorialState : ITutorialStateReadonly 
    {
        public int StepsCompleted;

        public event Action StepsCompletedChanged;

        int ITutorialStateReadonly.StepsCompleted => StepsCompleted;

        public void ChangeCompletedStep(int step)
        {
            StepsCompleted = step;    
            StepsCompletedChanged?.Invoke();
        }
    }

    public interface ITutorialStateReadonly
    {
        int StepsCompleted { get; }
        event Action StepsCompletedChanged;
    }
}