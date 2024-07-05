using System.Collections.Generic;
using Assets._Game.Core.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Coins.Scripts;
using Assets._Game.Gameplay.Vfx.Scripts;
using UnityEngine;

namespace Assets._Game.Gameplay._Coins.Factory
{
    [CreateAssetMenu(fileName = "Coin Factory", menuName = "Factories/Coins")]
    public class CoinFactory : GameObjectFactory, ICoinFactory
    {
        [SerializeField] private LootCoin _lootCoinPrefab;
        [SerializeField] private RewardCoin _rewardCoinPrefab;
        [SerializeField] private RewardCoinVFX _rewardCoinVFXPrefab;

        private readonly Queue<LootCoin> _lootCoinsPool = new Queue<LootCoin>();
        
        private IAudioService _audioService;
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public LootCoin GetLootCoin()
        {
            if (_lootCoinsPool.Count > 0)
            {
                var lootCoin = _lootCoinsPool.Dequeue();
                lootCoin.gameObject.SetActive(true);
                return lootCoin;
            }

            return CreateNewLootCoin();
        }

        public RewardCoin GetRewardCoin()
        {
            var rewardCoin = CreateGameObjectInstance(_rewardCoinPrefab);
            rewardCoin.OriginFactory = this;
            return rewardCoin;
        }
        
        public RewardCoinVFX GetRewardCoinVfx()
        {
            var rewardCoin = CreateGameObjectInstance(_rewardCoinVFXPrefab);
            rewardCoin.OriginFactory = this;
            rewardCoin.Construct(_audioService);
            return rewardCoin;
        }

        private LootCoin CreateNewLootCoin()
        {
            if (_lootCoinPrefab == null)
            {
                Debug.LogError("LootCoin prefab is not assigned in the CoinFactory.");
                return null;
            }

            var newLootCoin = CreateGameObjectInstance(_lootCoinPrefab);
            if (newLootCoin != null)
            {
                newLootCoin.OriginFactory = this;
            }
            else
            {
                Debug.LogError("Failed to instantiate LootCoin from prefab.");
            }
            return newLootCoin;
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
                Destroy(_lootCoinsPool.Dequeue().gameObject);
            }
            _lootCoinsPool.Clear();
        }
    }
}