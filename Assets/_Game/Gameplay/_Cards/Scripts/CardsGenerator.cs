using System;
using System.Collections.Generic;
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
            _cardsScreenPresenter.CardBought += GenerateCardsScreen;
        }

        private void GenerateCardsScreen(int amount)
        {
            List<int> cardsId = new List<int>(amount);
            for (int i = 0; i < amount; i++)
            {
                var type = SelectType();

                if ( _cardsConfigRepository.TryGetCardsByType(type, out List<CardConfig> cardsCollection))
                {
                    var randomIndex = _random.Next(0, cardsCollection.Count);
                    cardsId.Add(cardsCollection[randomIndex].Id);
                }
            }

            _userContainer.UpgradeStateHandler.AddCards(cardsId);
            
            foreach (var id in cardsId)
            {
                _logger.Log($"GENERATED CARD WITH ID: {id}");
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

        void IDisposable.Dispose()
        {
            _cardsScreenPresenter.CardBought -= GenerateCardsScreen;
        }
    }
}