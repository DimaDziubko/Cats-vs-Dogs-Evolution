using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsSummoningModel
    {
        public int CurrentLevel;
        public int MinSummoningLevel;
        public int MaxSummoningLevel;
        public string Progress;
        public float ProgressValue;
        public Dictionary<int, CardsSummoning> AllCardSummonings;
    }
}