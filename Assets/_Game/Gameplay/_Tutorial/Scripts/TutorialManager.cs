using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Factory;
using Assets._Game.Gameplay._Tutorial.Scripts;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialManager : ITutorialManager, IDisposable
    {
        private readonly TutorialPointersParent _pointersParent;
        private readonly IUserContainer _persistentData;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUIFactory _uiFactory;
        private ITutorialStateReadonly TutorialStateReadonly => _persistentData.State.TutorialState;

        private readonly Dictionary<int, TutorialPointerView> _activePointers = new Dictionary<int, TutorialPointerView>();
        
        public TutorialManager(
            TutorialPointersParent pointersParent,
            IUserContainer persistentData,
            IGameInitializer gameInitializer,
            IUIFactory uiFactory)
        {
            _pointersParent = pointersParent;
            _persistentData = persistentData;
            _gameInitializer = gameInitializer;
            _uiFactory = uiFactory;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TutorialStateReadonly.StepsCompletedChanged += OnStepCompleted;
            _pointersParent.Disable();
        }

        void IDisposable.Dispose()
        {
            TutorialStateReadonly.StepsCompletedChanged -= OnStepCompleted;
            _gameInitializer.OnPostInitialization -= Init;
            ClearAllPointers(); 
        }

        public void Register(ITutorialStep tutorialStep)
        {
            if(tutorialStep == null) return;
            var step = tutorialStep.GetTutorialStepData().Step;
            if(TutorialStateReadonly.CompletedSteps.Contains(step)) return;
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
            var tutorialData = tutorialStep.GetTutorialStepData();
            
            if (_activePointers.ContainsKey(tutorialData.Step) ||
                TutorialStateReadonly.CompletedSteps.Contains(tutorialData.Step)) return;

            _pointersParent.Enable();
            TutorialPointerView view = _uiFactory.GetTutorialPointer(_pointersParent.RectTransform);
            view.Show(tutorialData);
            
            _activePointers[tutorialData.Step] = view;
        }

        private void OnTutorialBroke(ITutorialStep tutorialStep) => 
            Break(tutorialStep.GetTutorialStepData().Step);

        private void OnStepComplete(ITutorialStep tutorialStep)
        {
            UnRegister(tutorialStep);
            var tutorialData = tutorialStep.GetTutorialStepData();
            foreach (var step in tutorialData.AffectedSteps)
            {
                _persistentData.TutorialStateHandler.CompleteTutorialStep(step);
            }
        }

        private void OnStepCompleted(int step) => Break(step);

        private void Break(int step)
        {
            if (_activePointers.TryGetValue(step, out var pointer) && pointer != null)
            {
                pointer.Hide();
                _activePointers.Remove(step);
            }
            
            if(_activePointers.Count == 0) _pointersParent.Disable();
        }
        
        private void ClearAllPointers()
        {
            foreach (var pointer in _activePointers.Values)
            {
                pointer.Hide();
            }
            
            _activePointers.Clear();
        }
    }
}
