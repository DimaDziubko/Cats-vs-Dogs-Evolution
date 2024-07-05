using System;
using System.Collections.Generic;
using Assets._Game.Common;

namespace Assets._Game.Gameplay._Timer.Scripts
{
    public interface ITimerService
    {
        void CreateTimer(TimerType type, TimerData data, Action completeAction = null);
        void RemoveTimer(TimerType type);
        GameTimer GetTimer(TimerType type);
        void StartTimer(TimerType type);
        void Stop(TimerType type);
    }

    public class TimerService : ITimerService
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Dictionary<TimerType, GameTimer> _timers = new Dictionary<TimerType, GameTimer>(1);
        
        public TimerService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void CreateTimer(TimerType type, TimerData data, Action completeAction = null)
        {
            if (_timers.ContainsKey(type))
            {
                _timers[type].Stop();
                _timers.Remove(type);
            }
            
            var timer = new GameTimer(_coroutineRunner);
            timer.Init(data, completeAction);
            _timers[type] = timer;
        }

        public void RemoveTimer(TimerType type)
        {
            if (_timers.ContainsKey(type))
            {
                _timers[type].Stop();
                _timers.Remove(type);
            }
        }
        
        public void StartTimer(TimerType type)
        {
            if (_timers.TryGetValue(type, out var timer))
            {
                timer.Start();
            }
        }
        
        public void Stop(TimerType type)
        {
            if (_timers.TryGetValue(type, out var timer))
            {
                timer.Stop();
            }
        }
        
        public GameTimer GetTimer(TimerType type)
        {
            if (_timers.TryGetValue(type, out var timer))
            {
                return timer;
            }

            return null;
        }
    }
}