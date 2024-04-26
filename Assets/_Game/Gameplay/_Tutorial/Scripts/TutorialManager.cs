using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialManager
    {
        void Initialize();
        public void Register(ITutorialStep tutorialStep);
        public void UnRegister(ITutorialStep tutorialStep);
    }

    public class TutorialManager : ITutorialManager
    {
        private readonly TutorialPointerView _pointerView;
        private readonly IPersistentDataService _persistentData;
        private ITutorialStateReadonly TutorialStateReadonly => _persistentData.State.TutorialState;

        private bool _inProgress;
        
        public TutorialManager(
            TutorialPointerView pointerView,
            IPersistentDataService persistentData)
        {
            _pointerView = pointerView;
            _persistentData = persistentData;
        }

        public void Initialize()
        {
            TutorialStateReadonly.StepsCompletedChanged += OnStepCompleted;
            _pointerView.Hide();
            _inProgress = false;
        }

        public void Register(ITutorialStep tutorialStep)
        {
            if(tutorialStep.TutorialStep == null) return;
            var step = tutorialStep.TutorialStep.GetTutorialStepData().Step;
            if(TutorialStateReadonly.StepsCompleted >= step) return;
            tutorialStep.ShowTutorialStep += ShowTutorialStep;
            tutorialStep.CompleteTutorialStep += OnStepComplete;
            tutorialStep.BreakTutorial += OnTutorialBroke;
        }

        public void UnRegister(ITutorialStep tutorialStep)
        {
            if(tutorialStep.TutorialStep == null) return;
            tutorialStep.ShowTutorialStep -= ShowTutorialStep;
            tutorialStep.CompleteTutorialStep -= OnStepComplete;
            tutorialStep.BreakTutorial -= OnTutorialBroke;
        }

        private void ShowTutorialStep(ITutorialStep tutorialStep)
        {
            if(_inProgress) return;
            var tutorialData = tutorialStep.TutorialStep.GetTutorialStepData();
            _pointerView.Show(tutorialData);
            _inProgress = true;
        }

        private void OnTutorialBroke(ITutorialStep tutorialStep) => 
            OnStepCompleted();

        private void OnStepComplete(ITutorialStep tutorialStep)
        {
            UnRegister(tutorialStep);
            var tutorialData = tutorialStep.TutorialStep.GetTutorialStepData();
            _persistentData.CompleteTutorialStep(tutorialData.Step);
        }
        
        private void OnStepCompleted()
        {
            _pointerView.Hide();
            _inProgress = false;
        }
    }
}
