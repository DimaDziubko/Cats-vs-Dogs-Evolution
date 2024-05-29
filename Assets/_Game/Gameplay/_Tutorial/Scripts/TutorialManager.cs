using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialManager
    {
        void Init();
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

        public void Init()
        {
            TutorialStateReadonly.StepsCompletedChanged += OnStepCompleted;
            _pointerView.Hide();
            _inProgress = false;
        }

        public void Register(ITutorialStep tutorialStep)
        {
            if(tutorialStep == null) return;
            var step = tutorialStep.GetTutorialStepData().Step;
            if(TutorialStateReadonly.StepsCompleted >= step) return;
            tutorialStep.Show += Show;
            tutorialStep.Complete += OnStepComplete;
            tutorialStep.Cancel += OnTutorialBroke;
        }

        public void UnRegister(ITutorialStep tutorialStep)
        {
            if(tutorialStep == null) return;
            tutorialStep.Show -= Show;
            tutorialStep.Complete -= OnStepComplete;
            tutorialStep.Cancel -= OnTutorialBroke;
        }

        private void Show(ITutorialStep tutorialStep)
        {
            if(_inProgress) return;
            var tutorialData = tutorialStep.GetTutorialStepData();
            _pointerView.Show(tutorialData);
            _inProgress = true;
        }

        private void OnTutorialBroke(ITutorialStep tutorialStep) => 
            Break();

        private void OnStepComplete(ITutorialStep tutorialStep)
        {
            UnRegister(tutorialStep);
            var tutorialData = tutorialStep.GetTutorialStepData();
            _persistentData.CompleteTutorialStep(tutorialData.Step);
        }
        
        private void OnStepCompleted(int step) => Break();

        private void Break()
        {
            _pointerView.Hide();
            _inProgress = false;
        }
    }
}
