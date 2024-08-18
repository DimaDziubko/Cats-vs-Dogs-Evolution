using System;
using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.DataPresenters.UnitBuilderDataPresenter;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._GameplayUI.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderViewController : 
        IUnitBuilder,
        IFoodConsumer, 
        IStartBattleListener,
        IPauseListener,
        IStopBattleListener
    {
        public event Action BuilderStarted;
        public event Action<int, bool> ChangeFood;

        private readonly GameplayUI _gameplayUI;
        private readonly BattleField _battleField;

        private readonly IUnitBuilderDataPresenter _presenter;
        private readonly IAudioService _audioService;
        private readonly ITutorialManager _tutorialManager;
        private readonly IMyLogger _logger;
        
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;

        private TutorialStep TutorialStep => UnitBuilderUI.TutorialStep;


        public UnitBuilderViewController(
            GameplayUI gameplayUI,
            BattleField battleField,
            IUnitBuilderDataPresenter presenter,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            IMyLogger logger)
        {
            _gameplayUI = gameplayUI;
            _battleField = battleField;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _logger = logger;
            _presenter = presenter;
        }

        void IStartBattleListener.OnStartBattle()
        {
            StartBuilder();
        }

        private void StartBuilder()
        {
            InitButtonTypes();

            Unsubscribe();
            Subscribe();

            BuilderStarted?.Invoke();
            _gameplayUI.Show();
            
            _tutorialManager.Register(TutorialStep);
        }

        private void InitButtonTypes()
        {
            int index = 0;
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UnitType = (UnitType)index;
                index++;
            }
        }

        private void DisableButtons()
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Disable();
            }
        }

        private void Unsubscribe()
        {
            BuilderStarted -= _presenter.OnBuilderStarted;
            _presenter.BuilderModelUpdated -= UpdateButtonsData;
        }

        private void Subscribe()
        {
            BuilderStarted += _presenter.OnBuilderStarted;
            _presenter.BuilderModelUpdated += UpdateButtonsData;
        }

        void IFoodListener.OnFoodBalanceChanged(int value)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UpdateButtonState(value);
            }
        }

        void IFoodListener.OnFoodGenerated() { }

        private void UpdateButtonsData(Dictionary<UnitType, UnitBuilderBtnModel> models)
        {
            if (models == null || models.Count == 0)
            {
                DisableButtons();
                return;
            }
            
            foreach (UnitBuildButton button in UnitBuilderUI.Buttons)
            {
                if (models.TryGetValue(button.UnitType, out var data))
                {
                    button.Initialize(this, data);
                }
            }
        }

        void IUnitBuilder.Build(UnitType type, int foodPrice)
        {
            TutorialStep.CompleteStep();
            
            _battleField.UnitSpawner.SpawnPlayerUnit(type);
            ChangeFood?.Invoke(foodPrice, false);

            PlayButtonSound();
        }

        void IUnitBuilder.OnButtonChangeState(ButtonState state)
        {
            if (state == ButtonState.Active)
            {
                TutorialStep.ShowStep();
            } 
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
        void IPauseListener.SetPaused(bool isPaused)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.SetPaused(isPaused);
            }
        }

        void IStopBattleListener.OnStopBattle()
        {
            _gameplayUI.Hide();

            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Hide();
            }
            
            Unsubscribe();

            TutorialStep.CancelStep();
            _tutorialManager.UnRegister(TutorialStep);

            DisableButtons();
        }
    }
}