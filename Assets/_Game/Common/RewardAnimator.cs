using System;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._Currencies;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Assets._Game.UI.Common.Header.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Common
{
    public class RewardAnimator : IRewardAnimator, IDisposable, IInitializable
    {
        private const int MIN_OBJECTS_AMOUNT = 6;
        private const int MAX_OBJECTS_AMOUNT = 40;

        private readonly ICoinFactory _coinFactory;
        private readonly IAudioService _audioService;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IHeader _header;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;

        public RewardAnimator(
            ICoinFactory coinFactory,
            IAudioService audioService,
            IUserContainer userContainer,
            IMyLogger logger,
            IHeader header)
        {
            _coinFactory = coinFactory;
            _audioService = audioService;
            _userContainer = userContainer;
            _logger = logger;
            _header = header;
        }

        void IInitializable.Initialize()
        {
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            _logger.Log("REWARD ANIMATOR INITIALIZED SUCCESSFULLY");
        }
        
        void IDisposable.Dispose()
        {
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
        }

        private void OnCurrenciesChanged(Currencies type, double delta, CurrenciesSource source)
        {
            switch (type)
            {
                case UI._Currencies.Currencies.Coins:
                    TryToShowCoinsRewardAnimation(delta, source);
                    break;
                case UI._Currencies.Currencies.Gems:
                    TryToShowGemsRewardAnimation(delta, source);
                    break;
            }
        }

        private void TryToShowGemsRewardAnimation(double delta, CurrenciesSource source)
        {
            
        }

        private void TryToShowCoinsRewardAnimation(double delta, CurrenciesSource source)
        {
            if(delta < 1) return;
            
            float ratio = 0;
            if(delta < Currencies.Coins)
                ratio = (float)(delta / Currencies.Coins);
            if (delta > Currencies.Coins) ratio = 1;

            _logger.Log($"Delta: {delta}, Ratio: {ratio}");
            
            switch (source)
            {
                case CurrenciesSource.None:
                    break;
                case CurrenciesSource.Shop:
                    PlayCoins(_header.CoinsWalletWorldPosition, ratio);
                    break;
                case CurrenciesSource.Battle:
                    PlayCoins(_header.CoinsWalletWorldPosition, ratio);
                    break;
                case CurrenciesSource.MiniShop:
                    PlayCoins(_header.CoinsWalletWorldPosition, ratio);
                    break;
                case CurrenciesSource.FreeGemsPack:
                    break;
                case CurrenciesSource.Upgrade:
                    break;
            }
        }


        public void PlayCoins(Vector3 animationTargetPoint, float coinsRatio)
        {
            int coinsToSpawn = CalculateCoinsToSpawn(coinsRatio);
            _logger.Log($"Coins to Spawn: {coinsToSpawn}");
            PlayCoinsSound();
            RewardCoinVFX coins = _coinFactory.GetRewardCoinVfx();
            coins.Init(Vector3.zero, animationTargetPoint, coinsToSpawn);
        }

        private void PlayCoinsSound() => 
            _audioService.PlayCoinAppearanceSFX();

        private int CalculateCoinsToSpawn(float ratio) => 
            Mathf.CeilToInt(Mathf.Lerp(MIN_OBJECTS_AMOUNT, MAX_OBJECTS_AMOUNT, ratio));
    }
}