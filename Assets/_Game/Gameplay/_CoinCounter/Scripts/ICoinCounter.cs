using System;

namespace Assets._Game.Gameplay._CoinCounter.Scripts
{
    public interface ICoinCounter
    {
        event Action<float> Changed;
        float Coins { get; }
        float CoinsRation { get; }
        float MaxCoinsPerBattle { set;}
        void AddCoins(float amount);
        void ChangeFactor(int factor);
        void Cleanup();
        void MultiplyCoins(int multiplier);
    }
}