﻿using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Services.UserContainer;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Core.Configs.Repositories._Cards
{
    public class CardsConfigRepository : ICardsConfigRepository
    {
        private readonly IUserContainer _userContainer;

        public CardsConfigRepository(IUserContainer userContainer) => 
            _userContainer = userContainer;

        public int MinSummoningLevel => 1;
        public int MaxSummoningLevel => _userContainer.GameConfig.SummoningConfig.Count;

        public CardsSummoning GetSummoning(int level) => 
            _userContainer.GameConfig.SummoningConfig[level];

        public int GetX1CardPrice() => _userContainer.GameConfig.CardPricingConfig.x1CardPrice;

        public int GetX10CardPrice() => _userContainer.GameConfig.CardPricingConfig.x10CardPrice;
        public int GetCardsRequiredForNextLevel(int level) => GetSummoning(level).CardsRequiredForLevel;

        public Dictionary<int, CardsSummoning> GetAllSummonings() => _userContainer.GameConfig.SummoningConfig;
        public bool TryGetCardsByType(CardType type, out List<CardConfig> cards)
        {
           return _userContainer.GameConfig.CardConfigsByType.TryGetValue(type, out cards);
        }
    }
}