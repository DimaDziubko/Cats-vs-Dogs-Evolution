using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsPresenter : ICardsPresenter
    {
        public event Action<int, CardModel> CardModelUpdated;
        
        public Dictionary<int, CardModel>  CardModels => _cardModels;

        private Dictionary<int, CardModel> _cardModels = new Dictionary<int, CardModel> ();

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;
        
        public CardsPresenter(
            IUserContainer userContainer,
            ICardsConfigRepository cardsConfigRepository)
        {
            _userContainer = userContainer;
            _cardsConfigRepository = cardsConfigRepository;
        }
        
        
        public void Init()
        {
            InitCardModels();
        }
        
        private void InitCardModels()
        {
            List<Card> boughtCards = CardsState.Cards;
            foreach (var card in boughtCards)
            {
                CardConfig config = _cardsConfigRepository.ForCard(card.Id);
                
                var model = new CardModel()
                {
                    Config = config,
                    Level =  $"Level {card.Level}",
                    Progress = $"{card.Count}/{config.GetUpgradeCount(card.Level)}",
                    ProgressValue = Mathf.Min(card.Count / config.GetUpgradeCount(card.Level), 1),
                };
                
                _cardModels.Add(card.Id, model);
            }

            _cardModels = _cardModels
                .OrderBy(x => x.Value.Config.Type == CardType.Legendary ? 0 : 
                    x.Value.Config.Type == CardType.Epic ? 1 : 
                    x.Value.Config.Type == CardType.Rare ? 2 : 3)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}