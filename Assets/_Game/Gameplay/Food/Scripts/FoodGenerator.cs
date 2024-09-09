using System;
using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.CustomKernel;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.UserContainer;
using _Game.UI._GameplayUI.Scripts;
using Assets._Game.Core.UserState;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Gameplay.Food.Scripts
{
    public interface IFoodGenerator
    {
        void Register(IFoodListener listener);
        void Unregister(IFoodListener listener);
    }

    public class FoodGenerator : 
        IInitializable,
        IDisposable, 
        IFoodGenerator,
        IGameTickable,
        IStartBattleListener,
        IStopBattleListener,
        IBattleSpeedListener

    {
        private const float LERP_SPEED_MULTIPLIER = 150f;
        
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IEconomyConfigRepository _economyConfig;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly ICommonItemsConfigRepository _commonConfig;

        private readonly List<IFoodListener> _listeners = new List<IFoodListener>(1);
        private readonly List<IFoodConsumer> _consumers = new List<IFoodConsumer>(1);

        private IUpgradeItemsReadonly UpgradeItems => _generalDataPool.AgeDynamicData.UpgradeItems;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;

        private float _defaultProductionSpeed;
        private float _productionSpeed;

        private int _foodAmount;
        private float _accumulatedFood;
        private float _smoothProgress;

        private readonly FoodPanel _panel;

        private int FoodAmount
        {
            get => _foodAmount;
            set
            {
                _foodAmount = value;
                Notify(_foodAmount);
            }
        }

        private void Notify(int value)
        {
            foreach (var listener in _listeners)
            {
                listener.OnFoodBalanceChanged(value);
            }
            
            _panel.OnFoodChanged(value);
        }

        public FoodGenerator(
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            GameplayUI gameplayUI,
            IGeneralDataPool generalDataPool,
            IUserContainer userContainer)
        {
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _economyConfig = configRepositoryFacade.EconomyConfigRepository;
            _panel = gameplayUI.FoodPanel;
            _logger = logger;
            _generalDataPool = generalDataPool;
            _userContainer = userContainer;
        }

        void IInitializable.Initialize() => 
            UpgradeItems.Changed += UpdateGeneratorData;

        void IDisposable.Dispose() => 
            UpgradeItems.Changed -= UpdateGeneratorData;

        public void Register(IFoodListener listener)
        {
            _listeners.Add(listener);
            if (listener is IFoodConsumer consumer)
            {
                _consumers.Add(consumer);
                consumer.ChangeFood += ChangeFood;
            }
        }

        public void Unregister(IFoodListener listener)
        {
            _listeners.Remove(listener);
            if (listener is IFoodConsumer consumer)
            {
                consumer.ChangeFood -= ChangeFood;
                _consumers.Remove(consumer);
            }
        }

        public void OnStartBattle() => StartGenerator();

        private void StartGenerator()
        {
            _panel.SetupIcon(_commonConfig.ForFoodIcon(RaceState.CurrentRace));

            _defaultProductionSpeed = UpgradeItems.GetItemData(UpgradeItemType.FoodProduction).Amount;

            FoodAmount = _economyConfig.GetInitialFoodAmount();

            _panel.UpdateFillAmount(0);
            _panel.OnFoodChanged(FoodAmount);
        }

        private void UpdateGeneratorData(UpgradeItemType type, UpgradeItemDynamicData data)
        {
            if (type == UpgradeItemType.FoodProduction)
            {
                _productionSpeed = data.Amount;
            }
        }

        void IGameTickable.Tick(float deltaTime)
        {
            _accumulatedFood += deltaTime * _productionSpeed;
            
            if (_accumulatedFood > 1f)
            {
                FoodAmount += (int)_accumulatedFood;
                
                foreach (var listener in _listeners)
                {
                    listener.OnFoodGenerated();
                }
                
                _smoothProgress = 1f;
                _panel.UpdateFillAmount(_smoothProgress);

                _accumulatedFood %= 1f;
                _smoothProgress = 0f;
            }
            else
            {
                _smoothProgress = Mathf.Lerp(_smoothProgress, _accumulatedFood % 1, 
                    deltaTime * LERP_SPEED_MULTIPLIER);
            }

            _panel.UpdateFillAmount(_smoothProgress);
        }

        private void ChangeFood(int delta, bool isPositive)
        {
            delta = isPositive ? delta : (delta * -1);
            if(!isPositive && delta > FoodAmount) return;
            FoodAmount += delta;
        }
        
        void IStopBattleListener.OnStopBattle()
        {
            _panel.UpdateFillAmount(0);
            _smoothProgress = 0;
            _accumulatedFood = 0;
        }

        void IBattleSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            _productionSpeed = _defaultProductionSpeed * speedFactor;
        }
    }
}