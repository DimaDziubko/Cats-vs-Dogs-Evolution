using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataPresenters.UnitUpgradePresenter
{
    public class StatsPopupPresenter : IStatsPopupPresenter, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IRaceChanger _raceChanger;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IGameInitializer _gameInitializer;
        private readonly IGeneralDataPool _dataPool;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;

        private readonly Dictionary<UnitType, StatsPopupModel> _models = new Dictionary<UnitType, StatsPopupModel>(3);

        public StatsPopupPresenter(
            IUserContainer userContainer,
            IUnitDataProvider unitDataProvider,
            IConfigRepositoryFacade configRepositoryFacade,
            IAgeNavigator ageNavigator,
            IRaceChanger raceChanger,
            IBattleNavigator battleNavigator,
            IGameInitializer gameInitializer,
            IGeneralDataPool dataPool)
        {
            _userContainer = userContainer;
            _unitDataProvider = unitDataProvider;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _raceChanger = raceChanger;
            _ageNavigator = ageNavigator;
            _battleNavigator = battleNavigator;
            _gameInitializer = gameInitializer;
            _dataPool = dataPool;
            _gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            CreateNewModels();
            TimelineState.OpenedUnit += OnUnitOpened;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _raceChanger.RaceChanged += OnRaceChanged;
            _battleNavigator.BattleChanged += OnBattleChanged;
            BoostData.Changed += OnBoostChanged;
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            TimelineState.OpenedUnit -= OnUnitOpened;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _raceChanger.RaceChanged -= OnRaceChanged;
            _battleNavigator.BattleChanged -= OnBattleChanged;
            BoostData.Changed -= OnBoostChanged;
        }

        public StatsPopupModel GetStatsPopupModelFor(UnitType type)
        {
            bool isModel =  _models.TryGetValue(type, out StatsPopupModel model);
            if (isModel) return model;
            return new StatsPopupModel() {IsStatsUnlocked = false};
        }

        public UnitType FindNextAvailableModel(UnitType currentType, bool forward, out bool isAvailable)
        {
            var unitTypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToArray();
            
            if (forward)
            {
                for (int i = Array.IndexOf(unitTypes, currentType) + 1; i < unitTypes.Length; i++)
                {
                    if (_models[unitTypes[i]].IsStatsUnlocked)
                    {
                        isAvailable = true;
                        return unitTypes[i];
                    }
                }
                
                isAvailable = false;
                return UnitType.Light;
            }

            for (int i = Array.IndexOf(unitTypes, currentType) - 1; i >= 0; i--)
            {
                if (_models[unitTypes[i]].IsStatsUnlocked)
                {
                    isAvailable = true;
                    return unitTypes[i];
                }
            }
                
            isAvailable = false;
            return UnitType.Light;
        }

        private void OnBattleChanged()
        {
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                string timelineNumberInfo = $"Timeline {TimelineState.TimelineId + 1}";
                IUnitData enemyUnitData  = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.BATTLE);
                _models[type].EnemyWarriorWarriorInfoItemModel =
                    CreateInfoModel(_commonConfig, enemyUnitData, timelineNumberInfo);
            }
        }


        private void OnRaceChanged() => CreateNewModels();
        private void OnAgeChanged() => CreateNewModels();
        private void OnBoostChanged(BoostSource _, BoostType __, float ___) => CreateNewModels();

        private void OnUnitOpened(UnitType type) => _models[type].IsStatsUnlocked = true;


        private void CreateNewModels()
        {
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                IUnitData playerUnitData  = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE);
                IUnitData enemyUnitData  = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.BATTLE);

                string timelineNumberInfo = $"Timeline {TimelineState.TimelineId + 1}";
                
                _models[type] = new StatsPopupModel()
                {
                    IsStatsUnlocked = TimelineState.OpenUnits.Contains(type),
                    WarriorName = playerUnitData.Name,
                    
                    PlayerWarriorWarriorInfoItemModel = CreateInfoModel(_commonConfig, playerUnitData, timelineNumberInfo),
                    EnemyWarriorWarriorInfoItemModel = CreateInfoModel(_commonConfig, enemyUnitData, timelineNumberInfo)
                };
            }
        }

        private WarriorInfoItemModel CreateInfoModel(ICommonItemsConfigRepository commonConfig, IUnitData unitData, string timelineNumberInfo)
        {
            return new WarriorInfoItemModel()
            {
                TimelineNumberInfo = timelineNumberInfo,
                Icon = unitData.Icon,

                StatInfoModels = new Dictionary<StatType, StatInfoModel>(2)
                {
                    {
                        StatType.Damage, new StatInfoModel()
                        {
                            StatIcon = commonConfig.GetUnitAttackIconFor(unitData.Race),
                            StatFullShownValue = unitData.Damage.ToFormattedString(1),
                            StatFullNewValue = unitData.Damage.ToFormattedString(1),
                            StatBoostValue = unitData.GetStatBoost(StatType.Damage).ToFormattedString(),
                            NeedAnimation = false
                        }
                    },

                    {
                        StatType.Health, new StatInfoModel()
                        {
                            StatIcon = commonConfig.GetUnitHealthIconFor(unitData.Race),
                            StatFullShownValue = unitData.GetUnitHealthForFaction(Faction.Player).ToFormattedString(1),
                            StatFullNewValue = unitData.GetUnitHealthForFaction(Faction.Player).ToFormattedString(1),
                            StatBoostValue = unitData.GetStatBoost(StatType.Health).ToFormattedString(),
                            NeedAnimation = false
                        }
                    },

                }
            };

        }
    }
}