using System;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public interface ICoinCounter
    {
        event Action<float> Changed;
        float Coins { get; }
        void AddCoins(float amount);
        void ChangeFactor(int factor);
        void Cleanup();
        void MultiplyCoins(int multiplier);
    }
}