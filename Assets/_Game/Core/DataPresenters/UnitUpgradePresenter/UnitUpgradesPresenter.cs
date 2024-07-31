using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core._UpgradesChecker;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Core.DataPresenters.UnitUpgradePresenter;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;
using Screen = _Game.UI._MainMenu.Scripts.Screen;

namespace _Game.Core.DataPresenters.UnitUpgradePresenter
{
    public class UnitUpgradesPresenter : IUnitUpgradesPresenter, IUpgradeAvailabilityProvider, IDisposable 
    {
        public event Action<Dictionary<UnitType, UnitUpgradeItemModel>> UpgradeUnitItemsUpdated;
        IEnumerable<Screen> IUpgradeAvailabilityProvider.AffectedScreens
        {
            get
            {
                yield return Screen.Upgrades;
                yield return Screen.UpgradesAndEvolution;
            }
        }
        bool IUpgradeAvailabilityProvider.IsAvailable =>  
            _models.Values.Any(model => model.ButtonState == ButtonState.Active && !model.IsBought);

        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _persistentData;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGeneralDataPool _dataPool;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IRaceChanger _raceChanger;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;

        private readonly Dictionary<UnitType, UnitUpgradeItemModel> _models 
            = new Dictionary<UnitType, UnitUpgradeItemModel>(3);

        public UnitUpgradesPresenter(
            IUserContainer persistentData,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IGeneralDataPool dataPool,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            IRaceChanger raceChanger)
        {
            _persistentData = persistentData;
            _logger = logger;
            _upgradesChecker = upgradesChecker;
            _dataPool = dataPool;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _raceChanger = raceChanger;
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
                _persistentData.PurchaseStateHandler.PurchaseUnit(type, price, CurrenciesSource.Upgrade);
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

        private void PrepareUnitUpgradeItemModels()
        {
            foreach (var unitItem in _dataPool.AgeStaticData.GetUnitUpgradeItems())
            {
                _models[unitItem.Key] = new UnitUpgradeItemModel()
                {
                    StaticData = unitItem.Value,
                    IsBought = TimelineState.OpenUnits.Contains(unitItem.Key),
                    ButtonState = Currency.Coins > unitItem.Value.Price 
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
            var model = _models[type];
            model.ButtonState = Currency.Coins > model.StaticData.Price 
                ? ButtonState.Active 
                : ButtonState.Inactive;
            model.IsBought = TimelineState.OpenUnits.Contains(type);
        }

        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source)
            => UpdateUnitItems();


        private void UpdateUnitItems()
        {
            foreach (var unitItem in _dataPool.AgeStaticData.GetUnitUpgradeItems())
            {
                UpdateUnitItem(unitItem.Key);
            }

            UpgradeUnitItemsUpdated?.Invoke(_models);
        }

        void IUnitUpgradesPresenter.OnUpgradesWindowOpened() => 
            UpgradeUnitItemsUpdated?.Invoke(_models);

        private void Cleanup() => _models.Clear();
    }
}