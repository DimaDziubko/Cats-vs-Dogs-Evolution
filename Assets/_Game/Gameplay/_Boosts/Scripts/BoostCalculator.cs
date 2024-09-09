using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.UserContainer;
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
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;
        
        public BoostsCalculator(
            IGeneralDataPool dataPool,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger)
        {
            _dataPool = dataPool;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _cardsConfigRepository = configRepositoryFacade.CardsConfigRepository;
            _logger = logger;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            CalculateCardBoosts();
            CardsState.CardsCollected += OnCardsCollected;
            CardsState.CardUpgraded += OnCardsUpgraded;
        }

        public void Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            CardsState.CardsCollected -= OnCardsCollected;
            CardsState.CardUpgraded -= OnCardsUpgraded;
        }

        private void OnCardsUpgraded(int _) => CalculateCardBoosts();

        private void OnCardsCollected(List<int> _) => CalculateCardBoosts();

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