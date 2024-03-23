using _Game.Gameplay._Coins.Scripts;

namespace _Game.Gameplay._Coins.Factory
{
    public interface ICoinFactory
    {
        LootCoin GetLootCoin();
        RewardCoin GetRewardCoin();
        void Reclaim(Coin coin);
    }
}