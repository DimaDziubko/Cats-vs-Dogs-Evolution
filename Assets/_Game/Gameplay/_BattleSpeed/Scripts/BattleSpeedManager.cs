using System.Collections.Generic;
using _Game.Core._GameListenerComposite;

namespace _Game.Gameplay._BattleSpeed.Scripts
{
    public class BattleSpeedManager : IBattleSpeedManager
    {
        public float CurrentSpeedFactor { get; private set; }
        
        private readonly List<IBattleSpeedListener> _listeners = new List<IBattleSpeedListener>();

        public void Register(IBattleSpeedListener listener)
        {
            _listeners.Add(listener);
        }

        public void Unregister(IBattleSpeedListener listener)
        {
            _listeners.Remove(listener);
        }
        
        public void SetSpeedFactor(float speedFactor)
        {
            CurrentSpeedFactor = speedFactor;
            
            foreach (var listener in _listeners)
            {
                listener.OnBattleSpeedFactorChanged(speedFactor);
            }
        }
    }
}