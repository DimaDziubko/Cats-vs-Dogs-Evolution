using System;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialStep
    {
        TutorialStep TutorialStep { get; }

        event Action<ITutorialStep> ShowTutorialStep;
        event Action<ITutorialStep> CompleteTutorialStep;
        event Action<ITutorialStep> BreakTutorial;
    }
}