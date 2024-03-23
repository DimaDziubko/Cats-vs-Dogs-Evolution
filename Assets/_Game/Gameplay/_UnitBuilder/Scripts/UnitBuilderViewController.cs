using System;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.GameplayUI.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderViewController : IUnitBuilder, IBaseDestructionHandler, IPauseHandler
    {
        public event Action BuilderStarted;
        
        private readonly GameplayUI _gameplayUI;
        private readonly BattleField _battleField;
        private readonly IFoodGenerator _foodGenerator;

        private readonly IAgeStateService _ageState;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;

        public bool IsPaused => _pauseManager.IsPaused;
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;

        public UnitBuilderViewController(
            GameplayUI gameplayUI, 
            BattleField battleField, 
            IFoodGenerator foodGenerator,
            IAgeStateService ageState,
            IAudioService audioService,
            IPauseManager pauseManager)
        {
            _gameplayUI = gameplayUI;
            _battleField = battleField;
            _foodGenerator = foodGenerator;
            _ageState = ageState;
            _audioService = audioService;
            _pauseManager = pauseManager;
        }
        
        public void StartBuilder()
        {
            Unsubscribe();
            Subscribe();

            _pauseManager.Register(this);
            
            BuilderStarted?.Invoke();
            _gameplayUI.Show();
            OnFoodChanged(_foodGenerator.FoodAmount);
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

            if(foodPrice > _foodGenerator.FoodAmount) return;
            _battleField.UnitSpawner.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);

            PlayButtonSound();
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }

        void IBaseDestructionHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            StopBuilder();
        }

        void IBaseDestructionHandler.OnBaseDestructionCompleted(Faction faction, Base @base) { }
        public void SetPaused(bool isPaused)
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
    }
}