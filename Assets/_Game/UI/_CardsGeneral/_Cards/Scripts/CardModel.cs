using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardModel
    {
        public CardConfig Config;
        public List<BoostItemModel> BoostItemModels;
        public string Progress;
        public string Level;
        public float ProgressValue;
        public bool IsNew;
        public bool IsNewShown;
        public bool IsGreatestType;
        public int UpgradeCount;
    }
}