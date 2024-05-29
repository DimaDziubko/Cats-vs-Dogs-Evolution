using System.Collections.Generic;
using System.Linq;
using _Game.Audio.Scripts;
using _Game.Core._SystemUpdate;
using _Game.Core.Factory;
using _Game.Core.Pause.Scripts;
using _Game.Core.Prefabs;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.Random;
using _Game.Creatives._LocalUnitConfigs;
using _Game.Creatives.Factories;
using _Game.Creatives.Scripts;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI.UnitBuilderBtn.Scripts;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace _Game.Creatives.Creative_1.Scenario
{
    public class CrSceneContext : MonoBehaviour
    {
        [SerializeField] private Sprite _foodSprite;
        
        [SerializeField] private CrHud _hud;
        
        [SerializeField] private SystemUpdate _systemUpdate;
        [SerializeField] private GameplayUI _gameplayUI;

        [SerializeField] private CoinFactory _coinFactory;
        [SerializeField] private CrVfxFactory _vfxFactory;
        [SerializeField] private CrUnitFactory _unitFactory;
        [SerializeField] private CrProjectileFactory _projectileFactory;
        [SerializeField] private BaseFactory _baseFactory;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _overlayCamera;

        //Audio
        [SerializeField] private SFXSourcesHolder _holder;
        [SerializeField] private SoundsHolder _soundsHolder;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioSource _musicSource;

        //UnitsConfig
        [SerializeField] private LocalUnitConfig[] _playerUnits;
        [SerializeField] private LocalUnitConfig[] _enemyUnits;

        [ShowInInspector]
        private readonly Dictionary<WeaponType, WeaponData> _playerWeaponsData = new Dictionary<WeaponType, WeaponData>();
        [ShowInInspector]
        private readonly Dictionary<WeaponType, WeaponData> _enemiesWeaponsData = new Dictionary<WeaponType, WeaponData>();

        [SerializeField] private int _initialFoodAmount = 0;
        [SerializeField] private float _foodProductionSpeed = 1;

        [SerializeField] private float _speedFactor = 1;


        [SerializeField] private Base _playerBase;
        [SerializeField] private Base _enemyBase;
        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;


        private FactoriesHolder _factories;
        private WorldCameraService _cameraService;
        private AudioService _audioService;
        private readonly PauseManager _pauseManager = new PauseManager();
        private readonly CoinCounter _coinCounter = new CoinCounter();
        private readonly BattleSpeedManager _battleSpeedManager = new BattleSpeedManager();
        private readonly UnityRandomService _random = new UnityRandomService();
        private CrFoodGenerator _foodGenerator;
        private CrUnitBuilderViewController _builder;
        public int InitialFood => _initialFoodAmount;
        public CrHud Hud => _hud;
        public IBattleSpeedManager BattleSpeedManager => _battleSpeedManager;
        public IRandomService RandomService => _random;
        public IUnitBuilder UnitBuilder => _builder;
        public IFoodGenerator FoodGenerator => _foodGenerator;
        public ICoinCounter CoinCounter => _coinCounter;
        public IAudioService AudioService => _audioService;
        public IPauseManager PauseManager => _pauseManager;
        public IFactoriesHolder FactoriesHolder => _factories;
        public IWorldCameraService CameraService => _cameraService;
        public GameplayUI GameplayUI => _gameplayUI;
        public ISystemUpdate SystemUpdate => _systemUpdate;
        public static CrSceneContext I { get; private set; }
        public float FoodProductionSpeed => _foodProductionSpeed;
        public UnitBuilderBtnData[] UnitBuilderButtonsData { get; private set; }

        private void Awake()
        {
            I = this;
            _cameraService = new WorldCameraService(_mainCamera, _overlayCamera);
            _audioService = new AudioService(_mixer, _holder, _musicSource, _soundsHolder);
            _factories = new FactoriesHolder(_unitFactory, _coinFactory, _vfxFactory, _baseFactory, _projectileFactory);
            _hud.Construct(_cameraService, _pauseManager, _audioService);
            _unitFactory.Initialize(_cameraService, _random, _audioService);
            _projectileFactory.Initialize(_audioService);
            _battleSpeedManager.SetSpeedFactor(_speedFactor);
            _foodGenerator = new CrFoodGenerator(_gameplayUI, _systemUpdate, _pauseManager, _battleSpeedManager);
            
            _coinCounter.Changed -= _hud.OnCoinsChanged;
            _coinCounter.Changed += _hud.OnCoinsChanged;

            SetupUnitBuilderBtnData();
            SetupWeaponData();
            
            SetupBasePosition();
            AstarPath.active.Scan();
        }

        private void SetupWeaponData()
        {
            foreach (var unit in _playerUnits)
            {
                if(unit.Data.Config.WeaponConfig.WeaponType == WeaponType.Melee) continue;
                WeaponData data = new WeaponData()
                {
                    Config = unit.Data.Config.WeaponConfig,
                    Layer = Constants.Layer.PLAYER_PROJECTILE,
                    MuzzlePrefab = unit.MuzzlePrefab,
                    ProjectilePrefab = unit.ProjectilePrefab,
                    ProjectileExplosionPrefab = unit.ProjectileExplosionPrefab
                };
                _playerWeaponsData.Add(unit.Data.Config.WeaponConfig.WeaponType, data);
            }
            
            foreach (var unit in _enemyUnits)
            {
                if(unit.Data.Config.WeaponConfig.WeaponType == WeaponType.Melee) continue;
                WeaponData data = new WeaponData()
                {
                    Config = unit.Data.Config.WeaponConfig,
                    Layer = Constants.Layer.ENEMY_PROJECTILE,
                    MuzzlePrefab = unit.MuzzlePrefab,
                    ProjectilePrefab = unit.ProjectilePrefab,
                    ProjectileExplosionPrefab = unit.ProjectileExplosionPrefab
                };
                _enemiesWeaponsData.Add(unit.Data.Config.WeaponConfig.WeaponType, data);
            }
        }

        private void SetupUnitBuilderBtnData()
        {
            UnitBuilderButtonsData = new UnitBuilderBtnData[3];
            int index = 0;

            foreach (var unit in _playerUnits)
            {
                var newData = new UnitBuilderBtnData()
                {
                    FoodIcon = _foodSprite,
                    FoodPrice = unit.Data.Config.FoodPrice,
                    Type = unit.Type,
                    UnitIcon = unit.Icon
                };

                UnitBuilderButtonsData[index] = newData;
                index++;
            }

            _builder = new CrUnitBuilderViewController(_gameplayUI, _foodGenerator, _audioService, _pauseManager);
        }

        public UnitData ForPlayerUnit(UnitType type)
        {
            LocalUnitConfig playerConfig = _playerUnits.First(x => x.Type == type);
            return playerConfig.Data;
        }
    
        public UnitData GetEnemy(UnitType type)
        {
            LocalUnitConfig playerConfig = _enemyUnits.First(x => x.Type == type);
            return playerConfig.Data;
        }

        private void SetupBasePosition()
        {
            CalculateBasePoints();
            
            if(_playerBase != null)
                _playerBase.Position = _playerBasePoint;
            if(_enemyBase != null)
                _enemyBase.Position = _enemyBasePoint;
        }
        
        private void CalculateBasePoints()
        {
            _enemyBasePoint = new Vector3(_cameraService.CameraWidth, 0, 0);
            _playerBasePoint = new Vector3(-_cameraService.CameraWidth, 0, 0);
        }

        public WeaponData ForPlayerWeapon(WeaponType type)
        {
            return _playerWeaponsData[type];
        }

        public WeaponData ForEnemyWeapon(WeaponType type)
        {
            return _enemiesWeaponsData[type];
        }
    }
}
