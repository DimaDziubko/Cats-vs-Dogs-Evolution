using _Game.Bundles.Bases.Factory;
using _Game.Bundles.Bases.Scripts;
using _Game.Bundles.Units.Common.Factory;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Common;
using _Game.Core.Services.Camera;
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

        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;
        
        private IUnitFactory _unitFactory;
        private IBaseFactory _baseFactory;
        private IWorldCameraService _cameraService;

        private readonly GameBehaviourCollection _playerUnits = new GameBehaviourCollection();
        private readonly GameBehaviourCollection _enemyUnits = new GameBehaviourCollection();

        private Base _enemyBase;
        private Base _playerBase;

        public bool IsEnemies => !_enemyUnits.IsEmpty;

        [Inject]
        public void Construct(
            IUnitFactory unitFactory,
            IBaseFactory baseFactory,
            IWorldCameraService cameraService)
        {
            _unitFactory = unitFactory;
            _baseFactory = baseFactory;
            _cameraService = cameraService;

            CalculateBasePoints();
        }


        public void UpdateEnemyBase()
        {
            if (_enemyBase != null)
            {
                _enemyBase.Recycle();
            }
            _enemyBase = _baseFactory.GetEnemyBase();
            _enemyBase.Position = _enemyBasePoint;
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

        public void UpdatePlayerBase()
        {
            if (_playerBase != null)
            {
                _playerBase.Recycle();
            }
            _playerBase = _baseFactory.GetPlayerBase();
            _playerBase.Position = _playerBasePoint;
            _playerBase.Rotation = Quaternion.Euler(0, 180f, 0);
        }

        private void CalculateBasePoints()
        {
            Vector3 screenEnemyBasePoint = new Vector3(Screen.width, Screen.height / 2f, 0);
            var worldEnemyBasePosition = _cameraService.ScreenToWorldPoint(screenEnemyBasePoint);
            _enemyBasePoint = new Vector3(worldEnemyBasePosition.x, worldEnemyBasePosition.y, 0);

            
            Vector3 screenPlayerBasePoint = new Vector3(0, Screen.height / 2f, 0);
            var worldPlayerBasePosition = _cameraService.ScreenToWorldPoint(screenPlayerBasePoint);
            _playerBasePoint = new Vector3(worldPlayerBasePosition.x, worldPlayerBasePosition.y, 0);
        }
    }
}
