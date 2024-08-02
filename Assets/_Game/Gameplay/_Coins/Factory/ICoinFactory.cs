using _Game.Gameplay.Vfx.Scripts;
using Assets._Game.Gameplay._Coins.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;

namespace _Game.Gameplay._Coins.Factory
{
    public interface ICoinFactory
    {
        LootCoin GetLootCoin();
        RewardCoinVFX GetRewardCoinVfx();
        void Reclaim(Coin coin);
    }
}