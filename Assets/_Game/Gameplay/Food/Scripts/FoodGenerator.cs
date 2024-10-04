using System.Collections.Generic;
using _Game.Core._DataProviders._FoodDataProvider;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.CustomKernel;
using _Game.UI._GameplayUI.Scripts;
using UnityEngine;

namespace _Game.Gameplay.Food.Scripts
{
    public interface IFoodGenerator
    {
        void Register(IFoodListener listener);
        void Unregister(IFoodListener listener);
    }

    public class FoodGenerator :
        IFoodGenerator,
        IGameTickable,
        IStartBattleListener,
        IStopBattleListener,
        IBattleSpeedListener

    {
        private const float LERP_SPEED_MULTIPLIER = 150f;
        
        private readonly IMyLogger _logger;
        private readonly IFoodProductionDataProvider _dataProvider;

        private readonly List<IFoodListener> _listeners = new List<IFoodListener>(1);
        private readonly List<IFoodConsumer> _consumers = new List<IFoodConsumer>(1);

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
            IMyLogger logger,
            GameplayUI gameplayUI,
            IFoodProductionDataProvider dataProvider)
        {
            _panel = gameplayUI.FoodPanel;
            _logger = logger;
            _dataProvider = dataProvider;
        }
        
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
            var data = _dataProvider.GetData();
            UpdateGeneratorData(data);
        }

        private void UpdateGeneratorData(IFoodProductionData data)
        {
            _panel.SetupIcon(data.FoodIcon);

            _defaultProductionSpeed = data.ProductionSpeed;

            FoodAmount = data.InitialFoodAmount;

            _panel.UpdateFillAmount(0);
            _panel.OnFoodChanged(FoodAmount);
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