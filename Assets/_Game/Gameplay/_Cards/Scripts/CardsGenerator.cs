using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Zenject;

namespace _Game.Gameplay._Cards.Scripts
{
    public interface ICardGenerator
    {
        
    }

    public class CardGenerator : ICardGenerator, IInitializable, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IRandomService _random;
        private readonly ICardsScreenPresenter _cardsScreenPresenter;
        private readonly IMyLogger _logger;

        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;

        public CardGenerator(
            IUserContainer userContainer,
            IConfigRepositoryFacade facade,
            IRandomService random,
            ICardsScreenPresenter cardsScreenPresenter,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _cardsConfigRepository = facade.CardsConfigRepository;
            _random = random;
            _cardsScreenPresenter = cardsScreenPresenter;
            _logger = logger;
        }
        
        void IInitializable.Initialize()
        {
            _cardsScreenPresenter.CardBought += GenerateCards;
        }

        void IDisposable.Dispose()
        {
            _cardsScreenPresenter.CardBought -= GenerateCards;
        }

        private void GenerateCards(int amount)
        {
            List<int> cardsId = new List<int>(amount);
            
            int initialDropListCount = _cardsConfigRepository.InitialDropList.Count;
            int playerProgressCount = CardsState.CardsSummoningProgressCount;
            
            for (int i = 0; i < amount; i++)
            {
                int currentIndex = playerProgressCount + i;
                
                if (currentIndex < initialDropListCount)
                {
                    _logger.Log($"Select card from initial drop list");
                    int cardId = _cardsConfigRepository.InitialDropList[currentIndex];
                    cardsId.Add(cardId);
                    continue;
                }
                
                if (_cardsConfigRepository.IsDropListEnabled)
                {
                    _logger.Log($"Select card from custom drop list");
                    int cardId = SelectCardFromDropList();
                    cardsId.Add(cardId);
                }
                else
                {
                    _logger.Log($"Select card with summoning");
                    int cardId = SelectCardWithSummoning();
                    cardsId.Add(cardId);
                }

            }

            _userContainer.UpgradeStateHandler.AddCards(cardsId);
            
            foreach (var id in cardsId)
            {
                _logger.Log($"GENERATED CARD WITH ID: {id}");
            }
        }

        private int SelectCardFromDropList()
        {
            int currentLevel = CardsState.CardsSummoningLevel;
            
            if (_cardsConfigRepository.TryGetSummoning(currentLevel, out var summoning))
            {
                var dropList = summoning.DropList;
                int dropListCount = dropList.Count;

                if (dropListCount == 0)
                {
                    _logger.Log($"DropList for level {currentLevel} is empty. Selecting card with summoning rates.");
                    return SelectCardWithSummoning();
                }
                
                int lastDropIdx = CardsState.LastDropIdx;
                
                int nextIndex;

                if (lastDropIdx >= dropListCount - 1 || lastDropIdx < 0)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex = lastDropIdx + 1;
                }
                
                int cardId = dropList[nextIndex];
                
                _userContainer.UpgradeStateHandler.UpdateLastDropIdx(nextIndex);

                return cardId;
            }
            
            _logger.Log($"No DropList found for level {currentLevel}, selecting card with summoning rates.");
            return SelectCardWithSummoning();
        }

        private int SelectCardWithSummoning()
        {
            var type = SelectType();

            if (_cardsConfigRepository.TryGetCardsByType(type, out List<CardConfig> cardsCollection) && cardsCollection.Count > 0)
            {
                float totalDropChance = cardsCollection.Sum(card => card.DropChance);

                if (totalDropChance <= 0)
                {
                    _logger.Log($"Total DropChance for type {type} is zero or negative. Selecting random card.");
                    var randomIndex = _random.Next(0, cardsCollection.Count);
                    return cardsCollection[randomIndex].Id;
                }
                
                float randomPoint = _random.GetValue() * totalDropChance;
                float cumulativeChance = 0f;

                foreach (var card in cardsCollection)
                {
                    cumulativeChance += card.DropChance;
                    if (randomPoint <= cumulativeChance)
                    {
                        return card.Id;
                    }
                }
                
                _logger.Log($"No card selected after DropChance calculation, returning first card of type {type}.");
                return cardsCollection[0].Id;
            }
            
            _logger.Log($"No cards found for type {type}, defaulting to Common type.");
            if (_cardsConfigRepository.TryGetCardsByType(CardType.Common, out var commonCards) && commonCards.Count > 0)
            {
                return commonCards.First().Id;
            }
            else
            {
                _logger.Log("No cards found at all. Returning default card ID 0.");
                return 0;
            }
        }

        private CardType SelectType()
        {
            float totalChance = 100;
            float randomPoint = _random.Next(0, totalChance);

            CardsSummoningModel model = _cardsScreenPresenter.CardsSummoningPresenter.CardsSummoningModel;
            CardsSummoning summoning = model.AllCardSummonings[model.CurrentLevel];

            float currentSum = 0;
            
            var dropChances = new List<(CardType Type, float Chance)>()
            {
                (CardType.Common, summoning.Common),
                (CardType.Rare, summoning.Rare),
                (CardType.Epic, summoning.Epic),
                (CardType.Legendary, summoning.Legendary)
            };
            
            foreach (var chance in dropChances)
            {
                currentSum += chance.Chance;
                if (randomPoint <= currentSum)
                {
                    return chance.Type;
                }
            }

            return CardType.Common;
        }
    }
}