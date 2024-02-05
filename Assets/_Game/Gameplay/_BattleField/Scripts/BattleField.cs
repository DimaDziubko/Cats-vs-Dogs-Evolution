using _Game.Common;
using _Game.Gameplay._Unit.Factory;
using _Game.Utils;
using UnityEngine;
using Zenject;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BattleField : MonoBehaviour
    {
        [FloatRangeSlider(0.5f, 0.7f)]
        private FloatRange _spawnerYRange = new FloatRange(0.5f, 0.7f);

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
        
        public void SpawnEnemy(int index)
        {
            var enemy = _unitFactory.GetForEnemy(index);
            
            enemy.Position = new Vector3(ENEMY_SPAWNER_X, _spawnerYRange.RandomValueInRange, 0);
            enemy.MovementDirection = Vector3.left;
            _enemyUnits.Add(enemy);
        }
        
        public void SpawnPlayerUnit(int id)
        {
            // var unit = _unitFactory.GetForPlayer(type);
            // unit.Position = 
            //     new Vector3(PLAYER_SPAWNER_X, _spawnerYRange.RandomValueInRange, 0);
            // unit.MovementDirection = Vector3.right;
            // unit.Rotation = Quaternion.Euler(0, 180f, 0);
            // _playerUnits.Add(unit);
        }

        public void Cleanup()
        {
            _playerUnits.Clear();
            _enemyUnits.Clear();
        }
        
    }
}
