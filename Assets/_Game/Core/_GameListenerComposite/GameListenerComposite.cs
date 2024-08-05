using System;
using System.Collections.Generic;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.Gameplay.Food.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Gameplay.GameResult.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core._GameListenerComposite
{
    public class GameListenerComposite : 
        MonoBehaviour, 
        IFoodConsumer,
        IStartBattleListener,
        IStopBattleListener,
        IPauseListener,
        IEndBattleListener,
        IBattleSpeedListener
    {
        public event Action<int, bool> ChangeFood;

        [ShowInInspector]
        [Inject]
        private IBattleManager _battleManager;
        
        [ShowInInspector]
        [Inject] 
        private List<IStartBattleListener> _startBattleListeners = new List<IStartBattleListener>();
        
        [ShowInInspector]
        [Inject] 
        private List<IPauseListener> _pauseListeners = new List<IPauseListener>();
        
        [ShowInInspector]
        [Inject] 
        private List<IStopBattleListener> _stopBattleListeners = new List<IStopBattleListener>();
        
        [ShowInInspector]
        [InjectLocal] 
        private List<IEndBattleListener> _endBattleListeners = new List<IEndBattleListener>();
        
        [ShowInInspector]
        [Inject]
        private IFoodGenerator _foodGenerator;

        [ShowInInspector]
        [InjectLocal] 
        private List<IFoodListener> _foodListeners = new List<IFoodListener>();

        [ShowInInspector]
        [Inject] 
        private List<IFoodConsumer> _consumers = new List<IFoodConsumer>();
        
        
        [ShowInInspector]
        [Inject]
        private IBattleSpeedManager _battleSpeedManager;
        
        [ShowInInspector]
        [Inject] 
        private List<IBattleSpeedListener> _battleSpeedListeners = new List<IBattleSpeedListener>();
        
        
        [Inject]
        private IMyLogger _logger;

        private void Start()
        {
            if(_battleManager.IsPaused) _battleManager.SetPaused(false);
            
            _foodGenerator.Register(this);
            foreach (var consumer in _consumers) consumer.ChangeFood += OnChangeFood;
            _battleManager.Register(this);
            _battleSpeedManager.Register(this);
        }

        private void OnDestroy()
        {
            _foodGenerator.Unregister(this);
            foreach (var consumer in _consumers) consumer.ChangeFood -= OnChangeFood;
            _battleManager.Unregister(this);
            _battleSpeedManager.Unregister(this);
        }

        void IFoodListener.OnFoodChanged(int value)
        {
            foreach (var it in _foodListeners)
            {
                if (it is { } listener)
                {
                    listener.OnFoodChanged(value);
                }
            }
        }

        private void OnChangeFood(int amount, bool isPositive)
        {
            ChangeFood?.Invoke(amount, isPositive);
        }

        public void OnStartBattle()
        {
            foreach (var it in _startBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStartBattle();
                }
            }
        }

        public void SetPaused(bool isPaused)
        {
            foreach (var it in _pauseListeners)
            {
                if (it is { } listener)
                {
                    listener.SetPaused(isPaused);
                }
            }
        }

        public void OnStopBattle()
        {
            foreach (var it in _stopBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStopBattle();
                }
            }
        }
        
        public void OnEndBattle(GameResultType result, bool wasExit)
        {
            foreach (var it in _endBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnEndBattle(result, wasExit);
                }
            }
        }

        public void OnBattleSpeedFactorChanged(float speedFactor)
        {
            foreach (var it in _battleSpeedListeners)
            {
                if (it is { } listener)
                {
                    listener.OnBattleSpeedFactorChanged(speedFactor);
                }
            }
        }
    }
}