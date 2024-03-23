using System;

namespace _Game.Gameplay.CoinCounter.Scripts
{
    public interface ICoinCounter
    {
        event Action<float> Changed;
        float Coins { get; }
        void AddCoins(float amount);
        void ChangeFactor(int factor);
        void Cleanup();
    }
}