using _Game.Creatives.Scripts;
using _Game.Gameplay._Units.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Creatives.Creative_1.Scenario
{
    public class CrQuickGame : MonoBehaviour
    {
        [SerializeField] private AudioClip _music;
        [SerializeField] private CrScenario _scenario;
        
        [SerializeField] private Transform[] _playerSpawnPoints;
        [SerializeField] private Transform[] _enemySpawnPoints;
        
        [SerializeField] private Transform[] _enemyDestination;
        [SerializeField] private Transform[] _playerDestination;
        
        public static CrQuickGame I { get; private set; }
        public bool BattleInProcess => _scenarioInProcess;

        private void Awake() => I = this;


        private CrBattleField _battleField;

        public Transform[] PlayerSpawnPoints => _playerSpawnPoints;
        public Transform[] EnemySpawnPoints => _enemySpawnPoints;
        
        public Transform[] EnemyDestination => _enemyDestination;
        public Transform[] PlayerDestination => _playerDestination;
        
        private void Start()
        {
            _battleField = new CrBattleField(
                CrSceneContext.I.CameraService,
                CrSceneContext.I.PauseManager,
                CrSceneContext.I.AudioService,
                CrSceneContext.I.CoinCounter,
                CrSceneContext.I.FactoriesHolder,
                CrSceneContext.I.BattleSpeedManager,
                CrSceneContext.I.Hud);
            
            _battleField.Init();
            _battleField.Cleanup();
        }

        private bool _scenarioInProcess;
        private CrScenario.State _activeScenario;

        [Button]
        public void StartNewGame()
        {
            _activeScenario = _scenario.Begin();
            _scenarioInProcess = true;
            CrSceneContext.I.FoodGenerator.StartGenerator();
            CrSceneContext.I.UnitBuilder.StartBuilder();
            CrSceneContext.I.CoinCounter.Cleanup();
            CrSceneContext.I.AudioService.Play(_music);
        }
        
        private void Update()
        {
            if(CrSceneContext.I.PauseManager.IsPaused || !_scenarioInProcess) return;

            if (_activeScenario.Progress())
            {
                _battleField.GameUpdate();
            }
            
            
            _foodAmount = CrSceneContext.I.FoodGenerator.FoodAmount;
        }

        public void SpawnEnemy(UnitType type)
        {
            _battleField.UnitSpawner.SpawnEnemy(type);
        }

        public void SpawnPlayerUnit(UnitType type)
        {
            _battleField.UnitSpawner.SpawnPlayerUnit(type);
        }

        [ShowInInspector] 
        private int _foodAmount;

        public Vector3 GetRandomEnemySpawnPoint()
        {
            var index = CrSceneContext.I.RandomService.Next(0, _enemySpawnPoints.Length);
            return _enemySpawnPoints[index].position;
        }

        public Vector3 GetRandomPlayerSpawnPoint()
        {
            var index = CrSceneContext.I.RandomService.Next(0, _playerSpawnPoints.Length);
            return _playerSpawnPoints[index].position;
        }

        public Vector3 GetRandomPlayerDestination()
        {
            if (_playerDestination != null && _playerDestination.Length > 0)
            {
                var index = CrSceneContext.I.RandomService.Next(0, _playerDestination.Length);
                return _playerDestination[index].position;
            }
            return  Vector3.zero;
        }

        public Vector3 GetRandomEnemyDestination()
        {
            if (_enemyDestination != null && _enemyDestination.Length > 0)
            {
                var index = CrSceneContext.I.RandomService.Next(0, _enemyDestination.Length);
                return _enemyDestination[index].position;
            }
            return  Vector3.zero;
        }
    }
}
