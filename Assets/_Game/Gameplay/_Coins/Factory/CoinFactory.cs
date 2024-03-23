using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Gameplay._Coins.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Coins.Factory
{
    [CreateAssetMenu(fileName = "Coin Factory", menuName = "Factories/Coins")]
    public class CoinFactory : GameObjectFactory, ICoinFactory
    {
        [SerializeField] private LootCoin _lootCoinPrefab;
        [SerializeField] private RewardCoin _rewardCoinPrefab;

        private readonly Queue<LootCoin> _lootCoinsPool = new Queue<LootCoin>();
        
        public LootCoin GetLootCoin()
        {
            LootCoin lootCoin;
            if (_lootCoinsPool.Count > 0)
            {
                lootCoin = _lootCoinsPool.Dequeue();
                lootCoin.gameObject.SetActive(true);
            }
            else
            {
                lootCoin = CreateGameObjectInstance(_lootCoinPrefab);
                lootCoin.OriginFactory = this;
            }
    
            return lootCoin;
        }

        public RewardCoin GetRewardCoin()
        {
            RewardCoin rewardCoin  = CreateGameObjectInstance(_rewardCoinPrefab);
            
            rewardCoin.OriginFactory = this;
            
            return rewardCoin;
        }

        public void Reclaim(Coin coin)
        {
            switch (coin)
            {
                case LootCoin lootCoin:
                    lootCoin.gameObject.SetActive(false);
                    _lootCoinsPool.Enqueue(lootCoin);
                    break;
                default:
                    Destroy(coin.gameObject);
                    break;
            }
        }

        public override void Cleanup()
        {
            while (_lootCoinsPool.Count > 0)
            {
                var lootCoin = _lootCoinsPool.Dequeue();
                Destroy(lootCoin.gameObject);
            }
            _lootCoinsPool.Clear();
        }
    }
}