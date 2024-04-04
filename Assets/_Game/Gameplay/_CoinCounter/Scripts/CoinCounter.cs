using System;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public class CoinCounter : ICoinCounter
    {
        public event Action<float> Changed;

        private float _coins;
        public float Coins => _coins;

        private int _factor;

        public CoinCounter(
            int factor = 1)
        {
            _factor = factor;
        }
        
        public void AddCoins(float amount)
        {
            _coins += amount * _factor;
            
            Changed?.Invoke(_coins);
        }

        public void ChangeFactor(int factor)
        {
            _factor = factor;
        }
        
        public void Cleanup()
        {
            _coins = 0;
        }

        public void MultiplyCoins(int multiplier)
        {
            _coins *= multiplier;
        }
    }
}