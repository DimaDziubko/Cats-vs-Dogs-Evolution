using System;
using _Game.Gameplay._Tutorial.Scripts;

namespace Assets._Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialStep
    {
        event Action<ITutorialStep> Show;
        event Action<ITutorialStep> Complete;
        event Action<ITutorialStep> Cancel;

        TutorialStepData GetTutorialStepData();
    }
}