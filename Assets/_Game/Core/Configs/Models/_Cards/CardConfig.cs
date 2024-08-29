using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils._Static;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._Cards
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Configs/Card")]
    public class CardConfig : ScriptableObject
    {
        public int Id;

        [ValueDropdown("GetCardTypes")]
        [OnValueChanged("UpdateColorBasedOnType")]
        public CardType Type;

        [ReadOnly]
        [ColorPalette] 
        public Color ColorIdentifier;

        public Sprite Icon;
        
        public bool IsNew;

        public int GetUpgradeCount(int level)
        {
            switch (level)
            {
                case 1:
                    return 2;
                case 2:
                    return 3;
                default:
                    return 4;
            }   
        }

        private void UpdateColorBasedOnType() => ColorIdentifier = GetColorForType(Type);

        private Color GetColorForType(CardType type) => CardColorMapper.GetColorForType(Type);

        private CardType[] GetCardTypes() => (CardType[])System.Enum.GetValues(typeof(CardType));
    }
}