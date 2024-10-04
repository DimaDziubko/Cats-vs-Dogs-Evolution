using System;
using System.Collections.Generic;
using _Game.Common;
using Assets._Game.Gameplay._Timer.Scripts;
using Sirenix.OdinInspector;

namespace _Game.Gameplay._Timer.Scripts
{
    public interface ITimerService
    {
        GameTimer CreateTimer(string key, TimerData data, Action completeAction = null);
        void RemoveTimer(string key);
        GameTimer GetTimer(string key);
        void StartTimer(string key);
        void Stop(string key);
    }

    public class TimerService : ITimerService
    {
        private readonly ICoroutineRunner _coroutineRunner;
        
        [ShowInInspector]
        private readonly Dictionary<string, GameTimer> _timers = new Dictionary<string, GameTimer>(1);

        public TimerService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public GameTimer CreateTimer(string key, TimerData data, Action completeAction = null)
        {
            if (_timers.ContainsKey(key))
            {
                _timers[key].Stop();
                _timers.Remove(key);
            }
            
            var timer = new GameTimer(_coroutineRunner);
            timer.Init(data, completeAction);
            _timers[key] = timer;
            return timer;
        }

        public void RemoveTimer(string key)
        {
            if (_timers.ContainsKey(key))
            {
                _timers[key].Stop();
                _timers.Remove(key);
            }
        }
        
        public void StartTimer(string key)
        {
            if (_timers.TryGetValue(key, out var timer))
            {
                timer.Start();
            }
        }
        
        public void Stop(string key)
        {
            if (_timers.TryGetValue(key, out var timer))
            {
                timer.Stop();
            }
        }
        
        public GameTimer GetTimer(string key)
        {
            if (_timers.TryGetValue(key, out var timer))
            {
                return timer;
            }

            return null;
        }
    }
}