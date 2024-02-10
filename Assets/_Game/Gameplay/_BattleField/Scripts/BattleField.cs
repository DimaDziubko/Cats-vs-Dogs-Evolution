using _Game.Bundles.Units.Common.Factory;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Common;
using _Game.Utils;
using UnityEngine;
using Zenject;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BattleField : MonoBehaviour
    {
        //TODO Fix const positions due to screen size
        [FloatRangeSlider(-0.4f, -0.8f)]
        private FloatRange _spawnerYRange = new FloatRange(-0.4f, -0.8f);

        private const float PLAYER_SPAWNER_X = -2.8f;
        private const float ENEMY_SPAWNER_X = +2.8f;

        private IUnitFactory _unitFactory;
        
        private readonly GameBehaviourCollection _playerUnits = new GameBehaviourCollection();
        private readonly GameBehaviourCollection _enemyUnits = new GameBehaviourCollection();
        
        public bool IsEnemies => !_enemyUnits.IsEmpty;

        [Inject]
        public void Construct(IUnitFactory unitFactory)
        {
            _unitFactory = unitFactory;
        }

        public void GameUpdate()
        {
            _playerUnits.GameUpdate();
            _enemyUnits.GameUpdate();
        }
        
        public void SpawnEnemy(UnitType type)
        {
            var enemy = _unitFactory.GetForEnemy(type);
            
            enemy.Position = new Vector3(ENEMY_SPAWNER_X, _spawnerYRange.RandomValueInRange, 0);
            enemy.MovementDirection = Vector3.left;
            _enemyUnits.Add(enemy);
        }
        
        public void SpawnPlayerUnit(UnitType type)
        {
            var unit = _unitFactory.GetForPlayer(type);
            unit.Position = new Vector3(PLAYER_SPAWNER_X, _spawnerYRange.RandomValueInRange, 0);
            unit.MovementDirection = Vector3.right;
            unit.Rotation = Quaternion.Euler(0, 180f, 0);
            _playerUnits.Add(unit);
        }

        public void Cleanup()
        {
            _playerUnits.Clear();
            _enemyUnits.Clear();
        }
        
    }
}
