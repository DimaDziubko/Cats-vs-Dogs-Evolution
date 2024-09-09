using System;
using System.Collections.Generic;
using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Global;
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
        IBattleSpeedListener,
        IUIListener,
        ISaveGameTrigger
    {
        public event Action<int, bool> ChangeFood;
        public event Action<bool> SaveGameRequested;


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

        [ShowInInspector] 
        [Inject] private IUINotifier _uiNotifier;

        [ShowInInspector]
        [Inject] 
        private List<IUIListener> _uiListeners = new List<IUIListener>();

        [Inject]
        private IMyLogger _logger;

        [ShowInInspector]
        [Inject]
        private IGameSaver _gameSaver;

        [ShowInInspector]
        [Inject] 
        private List<ISaveGameTrigger> _saveGameTriggers = new List<ISaveGameTrigger>();

        private void Start()
        {
            if(_battleManager.IsPaused) _battleManager.SetPaused(false);
            
            _foodGenerator.Register(this);
            foreach (var consumer in _consumers) consumer.ChangeFood += OnChangeFood;
            foreach (var trigger in _saveGameTriggers) trigger.SaveGameRequested += OnSaveGameRequested;
            _battleManager.Register(this);
            _battleSpeedManager.Register(this);
            _uiNotifier.Register(this);
            _gameSaver.Register(this);
        }

        private void OnDestroy()
        {
            _foodGenerator.Unregister(this);
            foreach (var consumer in _consumers) consumer.ChangeFood -= OnChangeFood;
            foreach (var trigger in _saveGameTriggers) trigger.SaveGameRequested -= OnSaveGameRequested;
            _battleManager.Unregister(this);
            _battleSpeedManager.Unregister(this);
            _uiNotifier.Unregister(this);
            _gameSaver.Unregister(this);
        }

        void IFoodListener.OnFoodBalanceChanged(int value)
        {
            foreach (var it in _foodListeners)
            {
                if (it is { } listener)
                {
                    listener.OnFoodBalanceChanged(value);
                }
            }
        }

        void IFoodListener.OnFoodGenerated()
        {
            foreach (var it in _foodListeners)
            {
                if (it is { } listener)
                {
                    listener.OnFoodGenerated();
                }
            }
        }

        private void OnChangeFood(int amount, bool isPositive) => ChangeFood?.Invoke(amount, isPositive);
        
        private void OnSaveGameRequested(bool isDebounced) => SaveGameRequested?.Invoke(isDebounced);

        void IStartBattleListener.OnStartBattle()
        {
            foreach (var it in _startBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStartBattle();
                }
            }
        }

        void IPauseListener.SetPaused(bool isPaused)
        {
            foreach (var it in _pauseListeners)
            {
                if (it is { } listener)
                {
                    listener.SetPaused(isPaused);
                }
            }
        }

        void IStopBattleListener.OnStopBattle()
        {
            foreach (var it in _stopBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStopBattle();
                }
            }
        }

        void IEndBattleListener.OnEndBattle(GameResultType result, bool wasExit)
        {
            foreach (var it in _endBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnEndBattle(result, wasExit);
                }
            }
        }

        void IBattleSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            foreach (var it in _battleSpeedListeners)
            {
                if (it is { } listener)
                {
                    listener.OnBattleSpeedFactorChanged(speedFactor);
                }
            }
        }

        void IUIListener.OnScreenOpened(GameScreen gameScreen)
        {
            foreach (var it in _uiListeners)
            {
                if (it is { } listener)
                {
                    listener.OnScreenOpened(gameScreen);
                }
            }
        }

        void IUIListener.OnScreenClosed(GameScreen gameScreen)
        {
            foreach (var it in _uiListeners)
            {
                if (it is { } listener)
                {
                    listener.OnScreenClosed(gameScreen);
                }
            }
        }
    }
}