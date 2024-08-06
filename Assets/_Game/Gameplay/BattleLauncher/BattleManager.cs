using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core.Debugger;
using Assets._Game.Core._Logger;
using Assets._Game.Gameplay.GameResult.Scripts;

namespace _Game.Gameplay.BattleLauncher
{
    public enum BattleState
    {
        None,
        Play,
        Stop,
        End
    }
    
    public sealed  class BattleManager : IBattleManager
    {
        private IBattleLauncher _handler;

        private readonly List<IBattleListener> _listeners = new List<IBattleListener>();
        private readonly IMyLogger _logger;

        public BattleState State { get; private set; }

        public bool IsPaused { get; private set; }

        public BattleManager(
            IMyDebugger debugger,
            IMyLogger logger)
        {
            State = BattleState.None;
            debugger.BattleManager = this;
            _logger = logger;
        }
        
        public void Register(IBattleListener listener) => 
            _listeners.Add(listener);

        public void Unregister(IBattleListener listener) => 
            _listeners.Remove(listener);

        public void StartBattle()
        {
            State = BattleState.Play;
            _logger.Log("MANAGER START BATTLE");
            
            foreach (var it in _listeners)
            {
                if (it is IStartBattleListener startBattleListener)
                {
                    startBattleListener.OnStartBattle();
                }
            }
        }

        public void SetPaused(bool isPaused)
        {
            IsPaused = isPaused;
            _logger.Log("MANAGER PAUSE BATTLE");
            
            foreach (var it in _listeners)
            {
                if (it is IPauseListener pauseListener)
                {
                    pauseListener.SetPaused(isPaused);
                }
            }
        }

        public void StopBattle()
        {
            State = BattleState.Stop;
            
            _logger.Log("MANAGER STOP BATTLE");
            
            foreach (var it in _listeners)
            {
                if (it is IStopBattleListener stopListener)
                {
                    stopListener.OnStopBattle();
                }
            }
        }
        
        public void EndBattle(GameResultType result, bool wasExit)
        {
            State = BattleState.End;
            
            _logger.Log("MANAGER END BATTLE");
            
            foreach (var it in _listeners)
            {
                if (it is IEndBattleListener endListener)
                {
                    endListener.OnEndBattle(result, wasExit);
                }
            }
        }
        
    }
}