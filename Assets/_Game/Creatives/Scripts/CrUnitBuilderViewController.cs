using _Game.Creatives.Creative_1.Scenario;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._GameplayUI.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Creatives.Creative_1.Scenario;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Creatives.Scripts
{
    public class CrUnitBuilderViewController : ICrUnitBuilder, IPauseHandler
    {
        private readonly GameplayUI _gameplayUI;
        private readonly ICrFoodGenerator _foodGenerator;
        private readonly CoinCounter _coinCounter;
        private readonly IAudioService _audioService;
        private readonly IPauseManager _pauseManager;

        private bool IsPaused => _pauseManager.IsPaused;
        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;

        public CrUnitBuilderViewController(
            GameplayUI gameplayUI,
            ICrFoodGenerator foodGenerator,
            CoinCounter coinCounter,
            IAudioService audioService,
            IPauseManager pauseManager)
        {
            _gameplayUI = gameplayUI;
            _foodGenerator = foodGenerator;
            _coinCounter = coinCounter;
            _audioService = audioService;
            _pauseManager = pauseManager;
        }

        public void StartBuilder()
        {
            Unsubscribe();
            Subscribe();

            _pauseManager.AddHandler(this);
            _gameplayUI.Show();

            if (CrSceneContext.I.IsCoinsLogic)
            {
                InitButtonsData();
            }
            else
            {
                UpdateButtonsData(CrSceneContext.I.UnitBuilderButtonsData);
            }
            OnFoodChanged(_foodGenerator.FoodAmount);
        }

        public void StopBuilder()
        {
            _gameplayUI.Hide();

            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Hide();
            }

            _pauseManager.RemoveHandler(this);

            Unsubscribe();

            DisableButtons();
        }

        public void Build(UnitType type, int foodPrice)
        {
            if (IsPaused) return;

            if (foodPrice > _foodGenerator.FoodAmount) return;
            CrQuickGame.I.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);

            PlayButtonSound();
        }

        public void Build(UnitType type, float coinPrice)
        {
            if (IsPaused) return;

            if (coinPrice > _coinCounter.Coins) return;
            CrQuickGame.I.SpawnPlayerUnit(type);
            _coinCounter.AddCoins(coinPrice * -1);

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

        private void UpdateButtonsData(UnitBuilderBtnModel[] builderData)
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
        private void InitButtonsData()
        {
            CrSceneContext.I.InitUnitButtons(this);
        }

        private void OnFoodChanged(int amount)
        {
            if (CrSceneContext.I.IsCoinsLogic)
                return;

            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.UpdateButtonState(amount);
            }
        }

        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();

        private void DisableButtons()
        {
            if (CrSceneContext.I.IsCoinsLogic)
                return;

            foreach (var button in UnitBuilderUI.Buttons)
            {
                button.Disable();
            }
        }
    }
}