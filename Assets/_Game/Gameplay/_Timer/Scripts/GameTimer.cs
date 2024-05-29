using System;
using System.Collections;
using _Game.Common;
using UnityEngine;

namespace _Game.Gameplay._Timer.Scripts
{
    public class GameTimer
    {
        public event Action<float> Tick;

        private readonly ICoroutineRunner _coroutineRunner;
        
        private Coroutine _activeCoroutine;
        private float _duration;
        private float _startValue;
        private bool _countdown;
        private float _timeLeft;
        private float _currentTime;
        
        private Action _completeAction;

        public GameTimer(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

        public void Init(TimerData data, Action completeAction = null)
        {
            _startValue = data.StartValue;
            _duration = data.Duration;
            _countdown = data.Countdown;
            _timeLeft = _duration;
            _currentTime = _countdown ? _startValue : 0;
            _completeAction = completeAction;
            Tick?.Invoke(_startValue);
        }
        
        public float TimeLeft => _timeLeft;

        public void Start() => 
            _activeCoroutine = _coroutineRunner.StartCoroutine(RunTimer());

        private IEnumerator RunTimer()
        {
            float endTime = _countdown ? 0 : _startValue + _duration;

            while ((_countdown ? _currentTime >= endTime : _currentTime <= endTime) && _timeLeft > 0)
            {
                Tick?.Invoke(_currentTime);
                yield return new WaitForSeconds(1.0f);
                _currentTime = _countdown ? _currentTime - 1.0f : _currentTime + 1.0f;
                _timeLeft = _countdown ? _currentTime : endTime - _currentTime;
            }
    
            if (_timeLeft <= 0)
            {
                _completeAction?.Invoke();
                Stop();
            }
        }

        public void Stop()
        {
            if (_activeCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }
        }
        
        public void Reset()
        {
            Stop();
            _currentTime = _countdown ? _startValue : 0;
            _timeLeft = _duration;
        }
    }
}