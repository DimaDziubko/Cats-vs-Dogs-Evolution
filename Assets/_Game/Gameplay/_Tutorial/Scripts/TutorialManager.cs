using System;
using Assets._Game.Core._GameInitializer;
using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;

namespace Assets._Game.Gameplay._Tutorial.Scripts
{
    public class TutorialManager : ITutorialManager, IDisposable
    {
        private readonly TutorialPointerView _pointerView;
        private readonly IUserContainer _persistentData;
        private readonly IGameInitializer _gameInitializer;
        private ITutorialStateReadonly TutorialStateReadonly => _persistentData.State.TutorialState;

        private bool _inProgress;

        public TutorialManager(
            TutorialPointerView pointerView,
            IUserContainer persistentData,
            IGameInitializer gameInitializer)
        {
            _pointerView = pointerView;
            _persistentData = persistentData;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TutorialStateReadonly.StepsCompletedChanged += OnStepCompleted;
            _pointerView.Hide();
            _inProgress = false;
        }

        void IDisposable.Dispose()
        {
            TutorialStateReadonly.StepsCompletedChanged -= OnStepCompleted;
            _gameInitializer.OnPostInitialization -= Init;
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
