using _Game.Core.Configs.Models;
using _Game.Core.Services.Battle;
using _Game.Core.Services.StaticData;
using _Game.Gameplay._BattleField.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Battle.Scripts
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private Image _environment;
        [SerializeField] private BattleField _battleField;
        
        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;
        private bool _scenarioInProcess;
        
        private IAssetProvider _assetProvide;
        private IBattleStateService _battleState;

        private int _currentBattleIndex;

        public void Construct(IBattleStateService battleState)
        {
            _battleState = battleState;
            
            _scenarioExecutor = new BattleScenarioExecutor();

            _battleState.BattleChanged += UpdateBattle;
            
            UpdateBattle();
        }

        public void StartBattle()
        {
            _scenarioInProcess = true;
            _activeScenario = _scenarioExecutor.Begin(_battleField);
        }

        private void UpdateBattle()
        {
            BattleConfig config = _battleState.GetCurrentBattleConfig();
            BattleAsset asset = _battleState.GetBattleAsset();
            
            _environment.sprite = asset.Environment;
            _scenarioExecutor.Init(_battleState.GetScenario()); 
            
            Debug.Log($"Battle updated with {config.Id} ");
        }

        private void OnDisable()
        {
            _battleState.BattleChanged -= UpdateBattle;
        }

        public void GameUpdate()
        {
            if (_scenarioInProcess)
            {
                if (_activeScenario.Progress() == false && !_battleField.IsEnemies)
                {
                    _scenarioInProcess = false;
                }

                _battleField.GameUpdate();
            }
        }

        public void Cleanup()
        {
            _battleField.Cleanup();
        }
    }
}