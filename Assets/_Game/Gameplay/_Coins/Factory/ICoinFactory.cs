using Assets._Game.Gameplay._Coins.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;

namespace Assets._Game.Gameplay._Coins.Factory
{
    public interface ICoinFactory
    {
        LootCoin GetLootCoin();
        RewardCoin GetRewardCoin();
        RewardCoinVFX GetRewardCoinVfx();
        void Reclaim(Coin coin);
    }
}