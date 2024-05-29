using _Game.Gameplay._Coins.Scripts;
using _Game.Gameplay.Vfx.Scripts;

namespace _Game.Gameplay._Coins.Factory
{
    public interface ICoinFactory
    {
        LootCoin GetLootCoin();
        RewardCoin GetRewardCoin();
        RewardCoinVFX GetRewardCoinVfx();
        void Reclaim(Coin coin);
    }
}