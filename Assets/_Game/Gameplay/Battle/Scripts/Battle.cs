using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Age.Scripts;
using _Game.Core.Services.Battle;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Battle.Scripts
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private Image _environment;
        [SerializeField] private BattleField _battleField;
        [SerializeField] private UnitBuilder _unitBuilder;
        
        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;

        private IBattleStateService _battleState;

        private int _currentBattleIndex;
        
        private FoodGenerator _foodGenerator;

        public bool ScenarioInProcess { get; private set; }

        public void Construct(
            IBattleStateService battleState,
            FoodGenerator foodGenerator,
            IAgeStateService ageState)
        {
            _battleState = battleState;
            _foodGenerator = foodGenerator;
            
            _scenarioExecutor = new BattleScenarioExecutor();
            
            _battleState.BattlePrepared += UpdateBattle;
            UpdateBattle(_battleState.BattleData);
            _battleField.UpdatePlayerBase();
            
            _unitBuilder.Construct(ageState);
        }
        
        public void StartBattle()
        {
            ScenarioInProcess = true;
            _activeScenario = _scenarioExecutor.Begin(_battleField);

            _unitBuilder.StartBuilder();
            _unitBuilder.UnitBuildRequested += OnUnitBuildRequested;
            _foodGenerator.FoodChanged += _unitBuilder.UpdateButtonsState;
            _unitBuilder.UpdateButtonsState(_foodGenerator.FoodAmount);
            
            //TODO Init player base health
        }

        public void GameUpdate()
        {
            if (ScenarioInProcess)
            {
                if (_activeScenario.Progress() == false && !_battleField.IsEnemies)
                {
                    ScenarioInProcess = false;
                }

                _battleField.GameUpdate();
            }
        } 

        public void Cleanup()
        {
            _battleField.Cleanup();
            _unitBuilder.UnitBuildRequested -= OnUnitBuildRequested;
            _unitBuilder.StopBuilder();
            _foodGenerator.FoodChanged -= _unitBuilder.UpdateButtonsState;
        }

        private void OnUnitBuildRequested(UnitType type, int foodPrice)
        {
            if(foodPrice > _foodGenerator.FoodAmount) return;
            _battleField.SpawnPlayerUnit(type);
            _foodGenerator.SpendFood(foodPrice);
        }

        private void UpdateBattle(BattleData data)
        {
            _environment.sprite = data.Environment;
            _battleField.UpdateEnemyBase();
            _scenarioExecutor.Init(data.Scenario);
        }

        private void OnDisable()
        {
            //TODO Check place
            _battleState.BattlePrepared -= UpdateBattle;
        }
    }
}