using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Game.Bundles.Bases.Scripts;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.Gameplay._UnitBuilder.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Age.Scripts
{
    public class AgeStateService : IAgeStateService
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
         private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;
        
        private readonly Dictionary<UnitType, UnitData> _playerUnitData = new Dictionary<UnitType, UnitData>(3);
        //private readonly List<UnitBuilderBtnData> _unitBuilderData = new List<UnitBuilderBtnData>(3);
        private readonly UnitBuilderBtnData[] _unitBuilderData = new UnitBuilderBtnData[3];

        private BaseData _playerBaseData;
        
        public Sprite GetCurrentFoodIcon { get; private set; }
        public BaseData GetForPlayerBase() => _playerBaseData;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        public AgeStateService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _assetProvider = assetProvider;
            _logger = logger;
        }

        public async UniTask Init()
        {
            TimelineState.OpenedUnit += OnOpenedUnit;
            await PrepareAgeData();
        }

        private async void OnOpenedUnit(UnitType type)
        {
            await AddPlayerUnitData(type);
            await AddUnitBuilderData(type);
        }

        public UnitData GetPlayerUnit(UnitType type)
        {
            if (_playerUnitData.TryGetValue(type, out var unitData))
            {
                return unitData;
            }
            else
            {
                _logger.Log("Player data is empty");
                return null;
            }
        }

        public UnitBuilderBtnData[] GetUnitBuilderData()
        {
            if (_unitBuilderData != null)
            {
                return _unitBuilderData;
            }
            else
            {
                _logger.Log("UnitBuilderData data is empty");
                return null;
            }
        }
        

        public async UniTask PrepareAgeData()
        {
            List<WarriorConfig> openPlayerUnitConfigs = _gameConfigController.GetOpenPlayerUnitConfigs();
            _logger.Log($"OpenPlayerUnits : {openPlayerUnitConfigs.Count}");

            var ct = _cts.Token;
            try
            {
                await PreparePlayerUnits(openPlayerUnitConfigs, ct);
                await PrepareUnitBuilderData(openPlayerUnitConfigs, ct);
                await PreparePlayerBaseData(ct);
                ct.ThrowIfCancellationRequested();
                
            }
            catch (OperationCanceledException)
            {
                _logger.Log("PrepareBattle was canceled.");
            }
        }
        
        private async UniTask PreparePlayerBaseData(CancellationToken ct)
        {
            var ageConfig = _gameConfigController.GetAgeConfig(TimelineState.AgeId);
            ct.ThrowIfCancellationRequested();
            var playerBasePrefab = await _assetProvider.Load<GameObject>(ageConfig.PlayerBaseKey);

            _playerBaseData = new BaseData()
            {
                BasePrefab = playerBasePrefab.GetComponent<Base>()
            };
        }

        private async UniTask PrepareUnitBuilderData(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            var foodIconKey = _gameConfigController.GetFoodIconKey();
            
            ct.ThrowIfCancellationRequested();
            var foodSprite = await _assetProvider.Load<Sprite>(foodIconKey);
            GetCurrentFoodIcon = foodSprite;

            for (int i = 0; i < openPlayerUnitConfigs.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var config = openPlayerUnitConfigs[i];
                var unitIcon = await _assetProvider.Load<Sprite>(config.IconKey);

                _logger.Log($"UnitBuilderData preparing {i}");
                
                var newData = new UnitBuilderBtnData
                {
                    Type = config.Type,
                    Food = foodSprite,
                    UnitIcon = unitIcon,
                    FoodPrice = config.FoodPrice,
                };
                
                _unitBuilderData[(int)config.Type] = newData; 
            }

            _logger.Log($"UnitBuilderData prepared {_unitBuilderData.Length}");
        }

        private async UniTask PreparePlayerUnits(List<WarriorConfig> openPlayerUnitConfigs, CancellationToken ct)
        {
            _playerUnitData.Clear();
    
            foreach (var config in openPlayerUnitConfigs)
            {
                ct.ThrowIfCancellationRequested();

                var go = await _assetProvider.Load<GameObject>(config.PlayerKey);
                var newData = new UnitData
                {
                    Config = config,
                    Prefab = go.GetComponent<Unit>()
                };

                _playerUnitData[config.Type] = newData;
            }

            _logger.Log("PlayerUnitData prepared");
        }
        
        
        private async UniTask AddPlayerUnitData(UnitType type)
        {
            var config = _gameConfigController.GetCurrentAgeUnits().FirstOrDefault(w => w.Type == type);
            if (config != null)
            {
                var prefab = await _assetProvider.Load<GameObject>(config.PlayerKey);
                var unitData = new UnitData
                {
                    Config = config,
                    Prefab = prefab.GetComponent<Unit>()
                };

                _playerUnitData[type] = unitData;
                _logger.Log($"Added new player unit data for {type}.");
            }
        }
        
        private async UniTask AddUnitBuilderData(UnitType type)
        {
            var config = _gameConfigController.GetCurrentAgeUnits().FirstOrDefault(w => w.Type == type);
            if (config != null)
            {
                var unitIcon = await _assetProvider.Load<Sprite>(config.IconKey);
                var newData = new UnitBuilderBtnData
                {
                    Type = type,
                    Food = GetCurrentFoodIcon,
                    UnitIcon = unitIcon,
                    FoodPrice = config.FoodPrice,
                };

                _unitBuilderData[(int)type] = newData;
                
                _logger.Log($"Added new unit builder data for {type}.");
            }
        }
    }
}