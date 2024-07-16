using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services.Audio;
using _Game.Creatives.Factories;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Audio.Scripts;
using Assets._Game.Core._SystemUpdate;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Core.Prefabs;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Core.Services.Random;
using Assets._Game.Creatives.Factories;
using Assets._Game.Creatives.LocalUnitConfigs.Scr;
using Assets._Game.Creatives.Scripts;
using Assets._Game.Gameplay._BattleSpeed.Scripts;
using Assets._Game.Gameplay._CoinCounter.Scripts;
using Assets._Game.Gameplay._Coins.Factory;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using Assets._Game.Gameplay.Food.Scripts;
using Assets._Game.UI.UnitBuilderBtn.Scripts;
using Assets._Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace _Game.Creatives.Creative_1.Scenario
{
    public class CrSceneContext : MonoBehaviour
    {
        [SerializeField] private Sprite _foodSprite;

        [SerializeField] private CrHud _hud;

        [SerializeField] private SystemUpdate _systemUpdate;

        [SerializeField] private GameplayUI _gameplayUI;
        [Space]
        [SerializeField] private bool _isCoinsLogic;
        [SerializeField] private int _startGold;
        [SerializeField] private CrGameplayUI _crGameplayUI;
        [Space]
        [SerializeField] private CoinFactory _coinFactory;
        [SerializeField] private CrVfxFactory _vfxFactory;
        [SerializeField] private CrUnitFactory _unitFactory;
        [SerializeField] private CrProjectileFactory _projectileFactory;
        [FormerlySerializedAs("towerFactory")][FormerlySerializedAs("_baseFactory")][SerializeField] private BaseFactory baseFactory;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _overlayCamera;

        //Audio
        [SerializeField] private SFXSourcesHolder _holder;
        [SerializeField] private SoundsHolder _soundsHolder;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SoundService _soundService;

        //UnitsConfig
        [SerializeField] private LocalUnitConfig[] _playerUnits;
        [SerializeField] private LocalUnitConfig[] _enemyUnits;

        [ShowInInspector]
        private readonly Dictionary<int, WeaponData> _playerWeaponsData = new Dictionary<int, WeaponData>();
        [ShowInInspector]
        private readonly Dictionary<int, WeaponData> _enemiesWeaponsData = new Dictionary<int, WeaponData>();

        [SerializeField] private int _initialFoodAmount = 0;
        [SerializeField] private float _foodProductionSpeed = 1;

        [SerializeField] private float _speedFactor = 1;


        [FormerlySerializedAs("playerTower")][FormerlySerializedAs("_playerBase")][SerializeField] private Base playerBase;
        [FormerlySerializedAs("enemyTower")][FormerlySerializedAs("_enemyBase")][SerializeField] private Base enemyBase;
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

        private ISoundService SoundService => _soundService;
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
        public UnitBuilderBtnModel[] UnitBuilderButtonsData { get; private set; }

        public bool IsCoinsLogic => _isCoinsLogic;

        private void Awake()
        {
            I = this;
            _cameraService = new WorldCameraService(_mainCamera, _overlayCamera);
            _audioService = new AudioService(_mixer, _holder, _musicSource, _soundsHolder);
            _factories = new FactoriesHolder(_unitFactory, _coinFactory, _vfxFactory, baseFactory, _projectileFactory);
            _hud.Construct(_cameraService, _pauseManager, _audioService);
            _unitFactory.Initialize(_cameraService, _random, SoundService);
            _projectileFactory.Initialize(SoundService);
            _battleSpeedManager.SetSpeedFactor(_speedFactor);
            //if (!_isCoinsLogic)
            _foodGenerator = new CrFoodGenerator(_gameplayUI, _systemUpdate, _pauseManager, _battleSpeedManager);
            //_soundService.Init();

            _coinCounter.Changed -= _hud.OnCoinsChanged;
            _coinCounter.Changed += _hud.OnCoinsChanged;
            if (_isCoinsLogic)
            {
                _coinCounter.AddCoins(_startGold);

                foreach (CrUnitBuilButton button in _crGameplayUI.CrUnitBuilButtons)
                {
                    _coinCounter.Changed += button.OnCoinsChanged;
                }
            }


            if (!_isCoinsLogic)
                SetupUnitBuilderBtnData();
            else
                SetupUnitBuilderCoinsBtnData();



            SetupWeaponData();

            SetupBasePosition();
            AstarPath.active.Scan();
        }

        private void SetupWeaponData()
        {
            foreach (var unit in _playerUnits)
            {
                if (unit.Data.Config.WeaponConfig.WeaponType == WeaponType.Melee) continue;
                WeaponData data = new WeaponData()
                {
                    Config = unit.Data.Config.WeaponConfig,
                    Layer = Constants.Layer.PLAYER_PROJECTILE,
                    MuzzlePrefab = unit.MuzzlePrefab,
                    ProjectilePrefab = unit.ProjectilePrefab,
                    ProjectileExplosionPrefab = unit.ProjectileExplosionPrefab
                };
                _playerWeaponsData.Add(unit.Data.Config.WeaponConfig.Id, data);
            }

            foreach (var unit in _enemyUnits)
            {
                if (unit.Data.Config.WeaponConfig.WeaponType == WeaponType.Melee) continue;
                WeaponData data = new WeaponData()
                {
                    Config = unit.Data.Config.WeaponConfig,
                    Layer = Constants.Layer.ENEMY_PROJECTILE,
                    MuzzlePrefab = unit.MuzzlePrefab,
                    ProjectilePrefab = unit.ProjectilePrefab,
                    ProjectileExplosionPrefab = unit.ProjectileExplosionPrefab
                };
                _enemiesWeaponsData.Add(unit.Data.Config.WeaponConfig.Id, data);
            }
        }
        private void SetupUnitBuilderCoinsBtnData()
        {
            int index = 0;
            foreach (var unit in _playerUnits)
            {
                _crGameplayUI.CrUnitBuilButtons[index].InitButtonData(unit.name, unit.Icon, unit.Data.Config.FoodPrice);
            }

            _builder = new CrUnitBuilderViewController(_gameplayUI, _foodGenerator, _coinCounter, _audioService, _pauseManager);
        }
        internal void InitUnitButtons(CrUnitBuilderViewController crUnitBuilderViewController)
        {
            for (int i = 0; i < _crGameplayUI.CrUnitBuilButtons.Length; i++)
            {
                _crGameplayUI.CrUnitBuilButtons[i].Initialize(crUnitBuilderViewController);
            }

        }
        private void SetupUnitBuilderBtnData()
        {
            UnitBuilderButtonsData = new UnitBuilderBtnModel[3];
            int index = 0;

            foreach (var unit in _playerUnits)
            {
                var newData = new UnitBuilderBtnModel()
                {
                    StaticData = new UnitBuilderBtnStaticData()
                    {
                        FoodPrice = unit.Data.Config.FoodPrice,
                        Type = unit.Type,
                        UnitIcon = unit.Icon
                    },

                    DynamicData = new UnitBuilderBtnDynamicData()
                    {
                        FoodIcon = _foodSprite,
                        IsUnlocked = true,
                    }

                };

                UnitBuilderButtonsData[index] = newData;
                index++;
            }

            _builder = new CrUnitBuilderViewController(_gameplayUI, _foodGenerator, _coinCounter, _audioService, _pauseManager);
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

            if (playerBase != null)
                playerBase.Position = _playerBasePoint;
            if (enemyBase != null)
                enemyBase.Position = _enemyBasePoint;
        }

        private void CalculateBasePoints()
        {
            _enemyBasePoint = new Vector3(_cameraService.CameraWidth, 0, 0);
            _playerBasePoint = new Vector3(-_cameraService.CameraWidth, 0, 0);
        }

        public WeaponData ForPlayerWeapon(int weaponId) =>
            _playerWeaponsData[weaponId];

        public WeaponData ForEnemyWeapon(int weaponId) =>
            _enemiesWeaponsData[weaponId];

    }
}
