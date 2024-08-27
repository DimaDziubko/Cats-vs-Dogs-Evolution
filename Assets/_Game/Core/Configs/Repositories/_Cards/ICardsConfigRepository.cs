﻿using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Core.Configs.Repositories._Cards
{
    public interface ICardsConfigRepository
    {
        int MinSummoningLevel { get; }
        int MaxSummoningLevel { get; }
        CardsSummoning GetSummoning(int level);
        int GetX1CardPrice();
        int GetX10CardPrice();
        int GetCardsRequiredForNextLevel(int level);
        Dictionary<int, CardsSummoning> GetAllSummonings();
        bool TryGetCardsByType(CardType type, out List<CardConfig> cards);
        
    }
}