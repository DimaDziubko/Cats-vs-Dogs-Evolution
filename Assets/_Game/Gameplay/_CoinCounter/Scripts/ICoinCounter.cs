using System;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public interface ICoinCounter
    {
        event Action<float> CoinsChanged;
        event Action<float> CoinsCollected;
        float Coins { get; }
        float CoinsRatio { get; }
        float MaxCoinsPerBattle { set;}
        void AddCoins(float amount);
        void ChangeFactor(int factor);
        void Cleanup();
        void MultiplyCoins(int multiplier);
    }
}