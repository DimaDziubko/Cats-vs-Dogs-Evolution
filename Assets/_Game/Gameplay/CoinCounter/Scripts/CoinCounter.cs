using System;

namespace _Game.Gameplay.CoinCounter.Scripts
{
    public class CoinCounter : ICoinCounter
    {
        public event Action<float> Changed;

        private float _score;
        public float Coins => _score;

        private int _factor;

        public CoinCounter(
            int factor = 1)
        {
            _factor = factor;
        }
        
        public void AddCoins(float amount)
        {
            _score += amount * _factor;
            
            Changed?.Invoke(_score);
        }

        public void ChangeFactor(int factor)
        {
            _factor = factor;
        }
        
        public void Cleanup()
        {
            _score = 0;
        }
    }
}