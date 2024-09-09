using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Core.DataPresenters.UnitUpgradePresenter
{
    public class UnitUpgradesPresenter : IUnitUpgradesPresenter, IUpgradeAvailabilityProvider, IDisposable
    {
        public event Action<Dictionary<UnitType, UnitUpgradeItemModel>> UpgradeUnitItemsUpdated;

        IEnumerable<GameScreen> IUpgradeAvailabilityProvider.AffectedScreens
        {
            get
            {
                yield return GameScreen.Upgrades;
                yield return GameScreen.UpgradesAndEvolution;
            }
        }

        bool IUpgradeAvailabilityProvider.IsAvailable =>
            _models.Values.Any(model => model.ButtonState == ButtonState.Active && !model.IsBought);

        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IRaceChanger _raceChanger;
        private readonly IMyLogger _logger;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IStatsPopupProvider _statsPopupProvider;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _userContainer.State.Currencies;


        private readonly Dictionary<UnitType, UnitUpgradeItemModel> _models
            = new Dictionary<UnitType, UnitUpgradeItemModel>(3);

        public UnitUpgradesPresenter(
            IUserContainer userContainer,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            IRaceChanger raceChanger,
            IUnitDataProvider unitDataProvider,
            IConfigRepositoryFacade configRepositoryFacade,
            IStatsPopupProvider statsPopupProvider)
        {
            _userContainer = userContainer;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _raceChanger = raceChanger;
            _unitDataProvider = unitDataProvider;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _statsPopupProvider = statsPopupProvider;
            
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            PrepareUnitUpgradeItemModels();
            _upgradesChecker.Register(this);
            TimelineState.OpenedUnit += OnUnitOpened;
            Currency.CurrenciesChanged += OnCurrenciesChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _raceChanger.RaceChanged += OnRaceChanged;
        }

        public void PurchaseUnit(UnitType type, float price)
        {
            if (Currency.Coins >= price)
            {
                _userContainer.PurchaseStateHandler.PurchaseUnit(type, price, CurrenciesSource.Upgrade);
            }
            else
            {
                Debug.LogError($"Cannot purchase unit {type}. Not enough coins or unit not found.");
            }
        }

        private void OnAgeChanged()
        {
            Cleanup();
            PrepareUnitUpgradeItemModels();
        }


        private void OnRaceChanged()
        {
            Cleanup();
            PrepareUnitUpgradeItemModels();
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            TimelineState.OpenedUnit -= OnUnitOpened;
            Currency.CurrenciesChanged -= OnCurrenciesChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _gameInitializer.OnMainInitialization -= Init;
            _raceChanger.RaceChanged -= OnRaceChanged;
        }

        async void IUnitUpgradesPresenter.ShowInfoFor(UnitType type)
        {
            var popup = await _statsPopupProvider.Load();
            bool isConfirmed = await popup.Value.ShowStatsAndAwaitForExit(type);
            if (isConfirmed)
            {
                popup.Value.Cleanup();
                popup.Dispose();
            }
        }

        private void PrepareUnitUpgradeItemModels()
        {
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                IUnitData unitData = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE);
                
                _models[type] = new UnitUpgradeItemModel()
                {
                    WarriorIcon = unitData.Icon,
                    Name = unitData.Name,
                    Price = unitData.Price,
                    Type = type,
                    
                    Stats = new Dictionary<StatType, StatInfoModel>()
                    {
                        {StatType.Damage, new StatInfoModel()
                        {
                            StatIcon = _commonConfig.GetUnitAttackIconFor(unitData.Race),
                            StatFullValue = unitData.Damage.ToFormattedString(1),
                            StatBoostValue = unitData.GetStatBoost(StatType.Damage).ToFormattedString(),
                        }},
                        
                        {StatType.Health, new StatInfoModel()
                        {
                            StatIcon = _commonConfig.GetUnitHealthIconFor(unitData.Race),
                            StatFullValue = unitData.GetUnitHealthForFaction(Faction.Player).ToFormattedString(1),
                            StatBoostValue = unitData.GetStatBoost(StatType.Health).ToFormattedString(),
                        }},
                    },
                        
                    IsBought = TimelineState.OpenUnits.Contains(type),
                    ButtonState = Currency.Coins > unitData.Price
                        ? ButtonState.Active
                        : ButtonState.Inactive
                };
            }
        }

        private void OnUnitOpened(UnitType type)
        {
            UpdateUnitItem(type);
            UpgradeUnitItemsUpdated?.Invoke(_models);
        }

        private void UpdateUnitItem(UnitType type)
        {
            IUnitData unitData = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE);
            
            var model = _models[type];
            model.ButtonState = Currency.Coins > model.Price
                ? ButtonState.Active
                : ButtonState.Inactive;
            model.IsBought = TimelineState.OpenUnits.Contains(type);

            model.Stats[StatType.Damage].StatIcon = _commonConfig.GetUnitAttackIconFor(unitData.Race);
            model.Stats[StatType.Damage].StatBoostValue = unitData.Damage.ToFormattedString();
            model.Stats[StatType.Damage].StatBoostValue = unitData.GetStatBoost(StatType.Damage).ToFormattedString();

            model.Stats[StatType.Health].StatIcon = _commonConfig.GetUnitHealthIconFor(unitData.Race);
            model.Stats[StatType.Health].StatFullValue =
                unitData.GetUnitHealthForFaction(Faction.Player).ToFormattedString();
            model.Stats[StatType.Health].StatBoostValue = unitData.GetStatBoost(StatType.Health).ToFormattedString();
        }

        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source)
            => UpdateUnitItems();


        private void UpdateUnitItems()
        {
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                UpdateUnitItem(type);
            }
            
            UpgradeUnitItemsUpdated?.Invoke(_models);
        }

        void IUnitUpgradesPresenter.OnUpgradesWindowOpened() =>
            UpgradeUnitItemsUpdated?.Invoke(_models);

        private void Cleanup() => _models.Clear();
    }
}