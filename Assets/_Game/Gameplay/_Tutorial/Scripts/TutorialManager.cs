using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Tutorial.Scripts;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialManager : ITutorialManager, IDisposable
    {
        private readonly TutorialPointerView _pointerView;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private ITutorialStateReadonly TutorialStateReadonly => _userContainer.State.TutorialState;

        private bool _inProgress;

        public TutorialManager(
            TutorialPointerView pointerView,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _pointerView = pointerView;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _logger = logger;
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
            if(TutorialStateReadonly.StepsCompleted >= step 
               || step > TutorialStateReadonly.StepsCompleted + 1 ) return;
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
            _logger.Log($"SHOW TUTORIAL STEP {tutorialData.Step}");
        }

        private void OnTutorialBroke(ITutorialStep tutorialStep)
        {
            Break();
            _logger.Log($"BREAK TUTORIAL STEP {tutorialStep.GetTutorialStepData().Step}");
        }

        private void OnStepComplete(ITutorialStep tutorialStep)
        {
            UnRegister(tutorialStep);
            var tutorialData = tutorialStep.GetTutorialStepData();
            _userContainer.TutorialStateHandler.CompleteTutorialStep(tutorialData.Step);
            _logger.Log($"COMPLETE TUTORIAL STEP {tutorialStep.GetTutorialStepData().Step}");
        }

        private void OnStepCompleted(int step) => Break();

        private void Break()
        {
            _pointerView.Hide();
            _inProgress = false;
        }
    }
}
