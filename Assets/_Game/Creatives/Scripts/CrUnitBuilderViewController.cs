using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UnitBuilderBtn.Scripts;

namespace _Game.Creatives.Scripts
{
    public class CrUnitBuilderViewController : IUnitBuilder, IPauseHandler
    {
        private readonly GameplayUI _gameplayUI;
        private readonly IFoodGenerator _foodGenerator;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;

        private bool IsPaused => _pauseManager.IsPaused;
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;

        public CrUnitBuilderViewController(
            GameplayUI gameplayUI,
            IFoodGenerator foodGenerator,
            IAudioService audioService,
            IPauseManager pauseManager)
        {
            _gameplayUI = gameplayUI;
            _foodGenerator = foodGenerator;
            _audioService = audioService;
            _pauseManager = pauseManager;
        }
        
        public void StartBuilder()
        {
            Unsubscribe();
            Subscribe();
            
            _pauseManager.Register(this);
            _gameplayUI.Show();

            UpdateButtonsData(CrSceneContext.I.UnitBuilderButtonsData);
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

            DisableButtons();
        }

        public void Build(UnitType type, int foodPrice)
        {
            if(IsPaused) return;
            
            if(foodPrice > _foodGenerator.FoodAmount) return;
            CrQuickGame.I.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);

            PlayButtonSound();
        }

        public void OnButtonChangeState(ButtonState state)
        {
            
        }

        void IPauseHandler.SetPaused(bool isPaused)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.SetPaused(isPaused);
            }
        }
        
        private void Unsubscribe()
        {
            _foodGenerator.FoodChanged -= OnFoodChanged;
            OnFoodChanged(_foodGenerator.FoodAmount);
        }

        private void Subscribe()
        {
            _foodGenerator.FoodChanged += OnFoodChanged;
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
        
        private void OnFoodChanged(int amount)
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UpdateButtonState(amount);
            }
        }
        
        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
        private void DisableButtons()
        {
            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Disable();
            }
        }
    }
}