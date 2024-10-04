using System;
using _Game.Core.Configs.Models;

namespace _Game.UI._Shop.Scripts
{
    public class FreeGemsPack
    {
        public event Action<bool> IsReadyChanged;
        public event Action<float> TimeChanged;
        public event Action<int> AmountChanged;
        
        public int Id;
        public FreeGemsPackConfig Config;
        public string Info => $"{_amount}/{Config.DailyGemsPackCount}";

        private bool _isReady;
        private float _remainingTime;
        private int _amount;

        public float RemainingTime => _remainingTime;
        
        public int Amount => _amount;
        public bool IsReady => _isReady;
        
        public void SetAmount(int amount)
        {
            if (_amount == amount)
            {
                return;
            }

            _amount = amount;
            AmountChanged?.Invoke(_amount); 
            SetReady(_amount > 0);
        }

        private void SetReady(bool isReady)
        {
            _isReady = isReady;
            IsReadyChanged?.Invoke(isReady);
        }

        public void Tick(float remainingTime)
        {
            _remainingTime = remainingTime;
            TimeChanged?.Invoke(_remainingTime);
        }
    }
}