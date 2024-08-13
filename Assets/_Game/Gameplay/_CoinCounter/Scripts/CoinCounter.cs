using System;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public class CoinCounter : ICoinCounter
    {
        public event Action<float> CoinsChanged;
        public event Action<float> CoinsCollected;

        private float _coins;
        public float Coins => _coins;

        public float CoinsRatio
        {
            get
            {
                if (_maxCoinsPerBattle > 0)
                {
                    return _coins / _maxCoinsPerBattle;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        private float _maxCoinsPerBattle;
        public float MaxCoinsPerBattle
        {
            set => _maxCoinsPerBattle = value;
        }
        
        private int _factor;

        public CoinCounter(
            int factor = 1)
        {
            _factor = factor;
        }
        
        public void AddCoins(float amount)
        {
            float delta = amount * _factor;
            _coins += delta;
            
            CoinsChanged?.Invoke(_coins);
            CoinsCollected?.Invoke(delta);
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