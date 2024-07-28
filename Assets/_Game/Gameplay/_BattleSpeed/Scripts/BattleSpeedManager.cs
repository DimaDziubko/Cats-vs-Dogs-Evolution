using System.Collections.Generic;

namespace _Game.Gameplay._BattleSpeed.Scripts
{
    public class BattleSpeedManager : IBattleSpeedManager
    {
        public float CurrentSpeedFactor { get; private set; }

        private readonly List<IBattleSpeedHandler> _handlers = new List<IBattleSpeedHandler>();
        
        public void Register(IBattleSpeedHandler handler)
        {
            _handlers.Add(handler);
        }

        public void UnRegister(IBattleSpeedHandler handler)
        {
            _handlers.Remove(handler);
        }

        public void SetSpeedFactor(float speedFactor)
        {
            CurrentSpeedFactor = speedFactor;
            
            foreach (var handler in _handlers)
            {
                handler.SetFactor(speedFactor);
            }
        }
    }
}