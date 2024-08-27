﻿using _Game.UI._CardsGeneral._Cards.Scripts;
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

        public bool IsNew;
        
        private void UpdateColorBasedOnType()
        {
            ColorIdentifier = GetColorForType(Type);
        }
        
        private Color GetColorForType(CardType type)
        {
            return CardColorMapper.GetColorForType(Type);
        }
        
        private CardType[] GetCardTypes()
        {
            return (CardType[])System.Enum.GetValues(typeof(CardType));
        }
    }
}