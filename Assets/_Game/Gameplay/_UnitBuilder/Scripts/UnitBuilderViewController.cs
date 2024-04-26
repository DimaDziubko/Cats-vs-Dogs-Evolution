using System;
using _Game.Core._Logger;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.GameplayUI.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderViewController : IUnitBuilder, IPauseHandler, ITutorialStep
    {
        public event Action BuilderStarted;
        
        private readonly GameplayUI _gameplayUI;
        private readonly BattleField _battleField;
        private readonly IFoodGenerator _foodGenerator;

        private readonly IAgeStateService _ageState;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;
        private readonly ITutorialManager _tutorialManager;
        private readonly IMyLogger _logger;

        private bool IsPaused => _pauseManager.IsPaused;
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;
        TutorialStep ITutorialStep.TutorialStep => UnitBuilderUI.TutorialStep;
        public event Action<ITutorialStep> ShowTutorialStep;
        public event Action<ITutorialStep> CompleteTutorialStep;
        public event Action<ITutorialStep> BreakTutorial;

        public UnitBuilderViewController(
            GameplayUI gameplayUI,
            BattleField battleField,
            IFoodGenerator foodGenerator,
            IAgeStateService ageState,
            IAudioService audioService,
            IPauseManager pauseManager,
            ITutorialManager tutorialManager,
            IMyLogger logger)
        {
            _gameplayUI = gameplayUI;
            _battleField = battleField;
            _foodGenerator = foodGenerator;
            _ageState = ageState;
            _audioService = audioService;
            _pauseManager = pauseManager;
            _tutorialManager = tutorialManager;
            _logger = logger;
        }

        public void StartBuilder()
        {
            Unsubscribe();
            Subscribe();

            _pauseManager.Register(this);

            BuilderStarted?.Invoke();
            _gameplayUI.Show();

            OnFoodChanged(_foodGenerator.FoodAmount);
            _tutorialManager.Register(this);
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

            BreakTutorial?.Invoke(this);
            _tutorialManager.UnRegister(this);

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
            BuilderStarted -= _ageState.OnBuilderStarted;
            _ageState.BuilderDataUpdated -= UpdateButtonsData;

            _foodGenerator.FoodChanged -= OnFoodChanged;
            OnFoodChanged(_foodGenerator.FoodAmount);
        }

        private void Subscribe()
        {
            BuilderStarted += _ageState.OnBuilderStarted;
            _ageState.BuilderDataUpdated += UpdateButtonsData;
            _foodGenerator.FoodChanged += OnFoodChanged;
        }

        private void OnFoodChanged(int amount)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UpdateButtonState(amount);
            }
        }

        private void UpdateButtonsData(UnitBuilderBtnData[] builderData)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Hide();
            }
            
            int dataIndex = 0;
            
            foreach (var button in UnitBuilderUI.Buttons)
            {
                var data = builderData[dataIndex];
                
                if (data != null)
                {
                    button.Initialize(this, data);
                }

                dataIndex++;
            }
        }

        void IUnitBuilder.Build(UnitType type, int foodPrice)
        {
            if(IsPaused) return;

            CompleteTutorialStep?.Invoke(this);
            
            if(foodPrice > _foodGenerator.FoodAmount) return;
            _battleField.UnitSpawner.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);

            PlayButtonSound();
        }

        void IUnitBuilder.OnButtonBecameInteractable()
        {
            ShowTutorialStep?.Invoke(this);   
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

    public interface IUnitBuilder
    {
        public void StartBuilder();
        public void StopBuilder();
        void Build(UnitType type, int foodPrice);
        void OnButtonBecameInteractable();
    }
}