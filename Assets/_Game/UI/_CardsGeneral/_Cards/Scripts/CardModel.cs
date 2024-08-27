using _Game.Core.Configs.Models._Cards;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardModel
    {
        public CardConfig Config;
        public Sprite Icon;
        public string Progress;
        public string Level;
        public float ProgressValue;
    }
}