using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.DataPresenters.UnitBuilderDataPresenter;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UnitBuilderBtn.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderViewController : IUnitBuilder, IPauseHandler
    {
        public event Action BuilderStarted;
        
        private readonly GameplayUI _gameplayUI;
        private readonly BattleField _battleField;
        private readonly IFoodGenerator _foodGenerator;
        
        private readonly IUnitBuilderDataPresenter _presenter;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;
        private readonly ITutorialManager _tutorialManager;
        private readonly IMyLogger _logger;

        private bool IsPaused => _pauseManager.IsPaused;
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;

        private TutorialStep TutorialStep => UnitBuilderUI.TutorialStep;
        
        public UnitBuilderViewController(
            GameplayUI gameplayUI,
            BattleField battleField,
            IFoodGenerator foodGenerator,
            IUnitBuilderDataPresenter presenter,
            IAudioService audioService,
            IPauseManager pauseManager,
            ITutorialManager tutorialManager,
            IMyLogger logger)
        {
            _gameplayUI = gameplayUI;
            _battleField = battleField;
            _foodGenerator = foodGenerator;
            _audioService = audioService;
            _pauseManager = pauseManager;
            _tutorialManager = tutorialManager;
            _logger = logger;
            _presenter = presenter;
        }

        public void StartBuilder()
        {
            InitButtonTypes();

            Unsubscribe();
            Subscribe();

            _pauseManager.Register(this);

            BuilderStarted?.Invoke();
            _gameplayUI.Show();

            OnFoodChanged(_foodGenerator.FoodAmount);
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

        public void StopBuilder()
        {
            _gameplayUI.Hide();

            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Hide();
            }

            _pauseManager.UnRegister(this);

            Unsubscribe();

            TutorialStep.CancelStep();
            _tutorialManager.UnRegister(TutorialStep);

            DisableButtons();
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

            _foodGenerator.FoodChanged -= OnFoodChanged;
            OnFoodChanged(_foodGenerator.FoodAmount);
        }

        private void Subscribe()
        {
            BuilderStarted += _presenter.OnBuilderStarted;
            _presenter.BuilderModelUpdated += UpdateButtonsData;
            _foodGenerator.FoodChanged += OnFoodChanged;
        }

        private void OnFoodChanged(int amount)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UpdateButtonState(amount);
            }
        }

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
            if(IsPaused) return;

            TutorialStep.CompleteStep();
            
            if(foodPrice > _foodGenerator.FoodAmount) return;
            _battleField.UnitSpawner.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);

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

        void IPauseHandler.SetPaused(bool isPaused)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.SetPaused(isPaused);
            }
        }
    }
}