using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.Core.UserState._State;

namespace _Game.Gameplay._Boosts.Scripts
{
    public interface IBoostsCalculator
    {
    }

    public class BoostsCalculator : IBoostsCalculator, IDisposable
    {
        private const float MIN_BOOST_VALUE = 1;
        
        private readonly IGeneralDataPool _dataPool;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IMyLogger _logger;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IIAPService _iapService;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;
        
        public BoostsCalculator(
            IGeneralDataPool dataPool,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IAgeNavigator ageNavigator,
            IIAPService iapService)
        {
            _dataPool = dataPool;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _cardsConfigRepository = configRepositoryFacade.CardsConfigRepository;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _iapService = iapService;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            CalculateCardBoosts();
            CalculateOfferBoosts();
            CardsState.CardsCollected += OnCardsCollected;
            CardsState.CardUpgraded += OnCardsUpgraded;
            _ageNavigator.AgeChanged += OnAgeChanged;
            Purchases.Changed += OnPurchasesChanged;
        }

        public void Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            CardsState.CardsCollected -= OnCardsCollected;
            CardsState.CardUpgraded -= OnCardsUpgraded;
            _ageNavigator.AgeChanged += OnAgeChanged;
            Purchases.Changed -= OnPurchasesChanged;
        }

        private void OnAgeChanged() => CalculateCardBoosts();

        private void OnCardsUpgraded(int _) => CalculateCardBoosts();

        private void OnCardsCollected(List<int> _) => CalculateCardBoosts();

        private void OnPurchasesChanged() => CalculateOfferBoosts();

        private void CalculateOfferBoosts() => CalculateProfitOfferBoosts();

        private void CalculateProfitOfferBoosts()
        {
            float coinsGained = MIN_BOOST_VALUE;
            var profitOffers = _iapService.ProfitOffers();
            foreach (var offer in profitOffers)
            {
                if (offer.IsActive)
                {
                    coinsGained *= offer.Config.CoinBoostFactor;
                }
            }
            
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Shop, BoostType.CoinsGained, coinsGained);
        }

        private void CalculateCardBoosts()
        {
            float allUnitsHealth = MIN_BOOST_VALUE;
            float allUnitsDamage = MIN_BOOST_VALUE;
            float coinsGained = MIN_BOOST_VALUE;
            float foodProduction = MIN_BOOST_VALUE;
            float baseHealth = MIN_BOOST_VALUE;

            foreach (var card in CardsState.Cards)
            {
                var config = _cardsConfigRepository.ForCard(card.Id);
                foreach (var boost in config.Boosts)
                {
                    switch (boost.Type)
                    {
                        case BoostType.None:
                            break;
                        case BoostType.AllUnitDamage:
                            allUnitsDamage *= boost.Exponential.GetValue(card.Level);
                            break;
                        case BoostType.AllUnitHealth:
                            allUnitsHealth *= boost.Exponential.GetValue(card.Level);
                            break;
                        case BoostType.FoodProduction:
                            foodProduction *= boost.Exponential.GetValue(card.Level);
                            break;
                        case BoostType.BaseHealth:
                            baseHealth *= boost.Exponential.GetValue(card.Level);
                            break;
                        case BoostType.CoinsGained:
                            coinsGained *= boost.Exponential.GetValue(card.Level);
                            break;
                    }
                }
            }
            
            _logger.Log($"AllUnitsHealth x{allUnitsHealth}");
            _logger.Log($"AllUnitsDamage x{allUnitsDamage}");
            _logger.Log($"Coins x{coinsGained}");
            _logger.Log($"Food Production x{foodProduction}");
            _logger.Log($"Base Health x{baseHealth}");
            
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Cards, BoostType.AllUnitDamage, allUnitsDamage);
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Cards, BoostType.CoinsGained, coinsGained);
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Cards, BoostType.FoodProduction, foodProduction);
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Cards, BoostType.AllUnitHealth, allUnitsHealth);
            _dataPool.AgeDynamicData.ChangeBoost(BoostSource.Cards, BoostType.BaseHealth, baseHealth);
        }
    }
}